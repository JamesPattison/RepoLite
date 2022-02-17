using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
using RepoLite.Common.Extensions;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using RepoLite.Common.Models.ProcedureParameters;
using RepoLite.Common.Models.Querying;
using RepoLite.Common.Options;
using Column = RepoLite.Common.Models.Column;
using Table = RepoLite.Common.Models.Table;

namespace RepoLite.DataAccess.Accessors
{
    public class MySqlAccess : DataSource
    {
        private SystemOptions _systemSettings;

        public MySqlAccess(
            IOptions<GenerationOptions> generationOptions,
            IOptions<SystemOptions> systemOptions)
            : base(generationOptions)
        {
            _systemSettings = systemOptions.Value;
        }

        public override IEnumerable<NameAndSchema> GetTables()
        {
            return GetTables(null);
        }

        public override IEnumerable<NameAndSchema> GetTables(string schema)
        {
            using (var conn = new MySqlConnection(_systemSettings.ConnectionString))
            {
                var tables = conn.Query<string>(@"
                    SELECT 
                        CONCAT(TABLE_SCHEMA, '.', TABLE_NAME)
                    FROM
                        INFORMATION_SCHEMA.TABLES
                    WHERE
                            TABLE_TYPE = 'BASE TABLE'
                        AND
                            (@schema IS NULL OR TABLE_SCHEMA = @schema)",
                    new {schema});

                var toReturn = tables.Select(table => table.GetTableAndSchema()).ToList();
                return toReturn;
            }
        }

        public override IEnumerable<ProcedureDefinition> GetProcedures()
        {
            using var cn = new MySqlConnection(_systemSettings.ConnectionString);

            var procedures = cn.Query<ProcedureDefinition>(@"
                            SELECT
                                   ROUTINE_SCHEMA as `Schema`,
                                   ROUTINE_NAME as Name,
                                   ROUTINE_DEFINITION as Definition
                            FROM INFORMATION_SCHEMA.ROUTINES");

            return procedures;
        }

        public override IEnumerable<ProcedureGenerationObject> LoadProcedures(IEnumerable<ProcedureDefinition> procedures)
        {
            using var cn = new MySqlConnection(_systemSettings.ConnectionString);
            
            foreach (var procedure in procedures)
            {
                var proc = new ProcedureGenerationObject
                {
                    Name = procedure.Name,
                    Schema = procedure.Schema,
                    Parameters = LoadProcedureParameters(procedure).ToList()
                };
                
                // look for commented out temporary tables
                var stringToInspect =
                    (!string.IsNullOrWhiteSpace(procedure.Definition) ? string.Join(" ", procedure.Definition.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).Where(x => x.StartsWith("#") || x.StartsWith("--")).Select(x => x.TrimStart('-', '#'))) : "") +
                    " " +
                    (!string.IsNullOrWhiteSpace(procedure.Definition) ? string.Join(" ", Regex.Matches(procedure.Definition, "/\\*.*?\\*/", RegexOptions.Singleline).Select(x => x.Value.Trim().TrimStart('/', '*').TrimEnd('*', '/').Replace("\n", " ").Replace("\r", " "))) : "");
                var temporaryTableMatches = Regex.Matches(stringToInspect, "create\\s+temporary\\s+table\\s+(?<tableName>[^\\s(]+)\\s*[(].+?[)]\\s*?[;]");
                foreach (Match match in temporaryTableMatches)
                {
                    var temporalTableName = match.Groups["tableName"].Value;
                    var temporalTableSql = match.Value;
                    string temporaryTableSql =
                        $"start transaction; drop temporary table if exists {temporalTableName}; {temporalTableSql} show columns from {temporalTableName}; rollback;";

                    var temporalTableParam = new TemporaryTableParameter
                    {
                        Name = temporalTableName,
                        Columns = new List<BasicParameter>(),
                        CreateSql = $"drop temporary table if exists {temporalTableName}; {temporalTableSql}"
                    }; //(temporalTableName, temporalTableName, temporalTableName, true)
                    // {
                    //     TvpColumns = new List<SqlProcColumn>(),
                    //     TemporalTableCreateSql = $"drop temporary table if exists {temporalTableName}; {temporalTableSql}"
                    // };
                    proc.Parameters.Add(temporalTableParam);
                    // ExecuteQueryMySql(temporaryTableSql,
                    //     row => { temporalTableParam.TvpColumns.Add(new SqlProcColumn(row.GetString(0), temporalTableParam.TvpColumns.Count, row.GetString(1), row.GetString(2) == "YES")); });
                    //
                    using var reader = cn.ExecuteReader(temporaryTableSql);

                    while (reader.Read())
                    {
                        var name = reader.GetString(0);
                        var columnType = reader.GetString(1);
                        var param = new BasicParameter
                        {
                            Name = name,
                            Pos = temporalTableParam.Columns.Count,
                            SqlDataType =
                                columnType.ToLower().StartsWith("bigint") || columnType.ToLower().StartsWith("int") ||
                                columnType.ToLower().StartsWith("smallint")
                                    ? columnType.ToLower().Split('(')[0]
                                    : columnType
                        };
                        param.Type = GetDataType(param.SqlDataType).Item2;
                        param.TypeString = GetDataType(param.SqlDataType).Item1;
                        temporalTableParam.Columns.Add(param);
                    }
                }

                proc.ResultSets = LoadProcedureResults(proc).ToList();
                yield return proc;
            }
        }

        private IEnumerable<IProcedureParameter> LoadProcedureParameters(NameAndSchema procedure)
        {
            using var cn = new MySqlConnection(_systemSettings.ConnectionString);

            var parameterInfo = cn.Query<ProcedureParameterInfo>(@"
                                select p.PARAMETER_NAME as Name,
                                       p.ORDINAL_POSITION as Pos,
                                       p.DATA_TYPE as SqlDataType,
                                       p.PARAMETER_MODE = 'OUT' or p.PARAMETER_MODE = 'INOUT' as IsOutput,
                                       0 as IsNullable,
                                       p.CHARACTER_MAXIMUM_LENGTH as Length,
                                       p.NUMERIC_PRECISION as `Precision`,
                                       p.NUMERIC_SCALE as Scale
                                from information_schema.ROUTINES r
                                join information_schema.PARAMETERS p on p.SPECIFIC_NAME = r.ROUTINE_NAME and p.SPECIFIC_SCHEMA = r.ROUTINE_SCHEMA
                                where
                                       r.ROUTINE_SCHEMA = @schema
                                    and
                                        r.ROUTINE_NAME = @name",
                new
                {
                    schema = procedure.Schema,
                    name = procedure.Name
                });

            foreach (var parameter in parameterInfo)
            {
                parameter.TypeString = GetDataType(parameter.SqlDataType).Item1;
                parameter.Type = GetDataType(parameter.SqlDataType).Item2;

                yield return parameter;
            }
        }

        private IEnumerable<ResultsSet> LoadProcedureResults(ProcedureGenerationObject procedureGenerationObject)
        {
            using var cn = new MySqlConnection(_systemSettings.ConnectionString);

            var paramString = "";
            var declares = "";
            foreach (var parameter in procedureGenerationObject.Parameters)
            {
                switch (parameter)
                {
                    case BasicParameter basic:
                        paramString += basic.Name.ToLower().StartsWith("caller_id")
                            ? "'11111111-1111-1111-1111-111111111111',"
                            : $"{basic.Type.GetDefault()},";

                        break;
                    case TableTypeParameter table:
                        // Not supported (yet)
                        // declares += $"DECLARE @tbl{table.Name} {table.SqlName};";
                        // paramString += $"{table.Name}=@tbl{table.Name}";
                        break;
                }
            }

            paramString = paramString.TrimEnd(',');
            var adapter = new MySqlDataAdapter($@"CALL {procedureGenerationObject.Name}({paramString});", cn);

            var customers = new DataSet();
            adapter.Fill(customers);

            foreach (DataTable table in customers.Tables)
            {
                var resultsSet = new ResultsSet {Values = new List<ResultsSetValue>()};
                foreach (DataColumn column in table.Columns)
                {
                    resultsSet.Values.Add(new ResultsSetValue
                    {
                        Name = column.ColumnName,
                        Type = column.DataType
                    });
                }

                yield return resultsSet;
            }
        }

        public override IEnumerable<Column> LoadTableColumns(Table table)
        {
            using (var cn = new MySqlConnection(_systemSettings.ConnectionString))
            {
                var columns = cn.Query<Column>(@"
                            SELECT
                                c.COLUMN_NAME AS DbColumnName,
                                0                                                                           as IsComputed,
                                c.DATA_TYPE                                                                 AS SqlDataType,
                                c.IS_NULLABLE = 'YES'                                                       AS IsNullable,
                                c.EXTRA like '%auto_increment%'                                             AS IsIdentity,
                                CASE
                                    WHEN c.COLUMN_DEFAULT LIKE '((%' AND c.COLUMN_DEFAULT LIKE '%))'
                                        THEN SUBSTRING(c.COLUMN_DEFAULT,3,length(c.COLUMN_DEFAULT)-4)
                                    WHEN c.COLUMN_DEFAULT LIKE '(%' AND c.COLUMN_DEFAULT LIKE '%)'
                                        THEN SUBSTRING(c.COLUMN_DEFAULT,2,length(c.COLUMN_DEFAULT)-2)
                                    ELSE c.COLUMN_DEFAULT
                                END                                                                         AS DefaultValue,
                                IF(x.COLUMN_NAME = c.COLUMN_NAME AND CONSTRAINT_TYPE = 'PRIMARY KEY', 1, 0) AS PrimaryKey,
                                IF(x.COLUMN_NAME = c.COLUMN_NAME AND CONSTRAINT_TYPE = 'FOREIGN KEY', 1, 0) AS ForeignKey,
                                x.REFERENCED_TABLE_NAME                                                     AS ForeignKeyTargetTable,
                                x.REFERENCED_COLUMN_NAME                                                    AS ForeignKeyTargetColumn,
                                COALESCE(c.CHARACTER_MAXIMUM_LENGTH, 0)                                     AS MaxLength,
                                COALESCE(c.NUMERIC_PRECISION, 0)                                            AS MaxIntLength,
                                COALESCE(c.NUMERIC_SCALE, 0)                                                AS MaxDecimalLength
                            FROM
                                information_schema.COLUMNS c
                                LEFT JOIN (SELECT
                                        ccu.TABLE_SCHEMA,
                                        ccu.TABLE_NAME,
                                        ccu.COLUMN_NAME,
                                        tc.CONSTRAINT_TYPE,
                                        ccu.REFERENCED_TABLE_NAME,
                                        ccu.REFERENCED_COLUMN_NAME
                                    FROM
                                        INFORMATION_SCHEMA.KEY_COLUMN_USAGE ccu
                                        INNER JOIN
                                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                                            ON
                                                ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                                                AND ccu.TABLE_SCHEMA = tc.TABLE_SCHEMA
                                                AND ccu.TABLE_NAME = tc.TABLE_NAME) x
                                    ON x.TABLE_SCHEMA = c.TABLE_SCHEMA
                                        AND	x.TABLE_NAME = c.TABLE_NAME
                                        AND x.COLUMN_NAME = c.COLUMN_NAME
                            where c.TABLE_NAME = @table
                                  and database() = @schema
                            ORDER BY 
	                            c.ORDINAL_POSITION ASC",
                    new

                    {
                        table = $"{table.DbTableName}".ToLower(),
                        schema = $"{table.Schema}".ToLower()
                    }).ToList();

                var toReturn = new List<Column>();

                foreach (var column in columns)
                {
                    var preExisting = toReturn.FirstOrDefault(x => x.DbColumnName == column.DbColumnName);
                    if (preExisting != null)
                    {
                        preExisting.ForeignKey |= column.ForeignKey;
                        preExisting.PrimaryKey |= column.PrimaryKey;
                    }
                    else
                    {
                        var dt = GetDataType(column.SqlDataType);
                        column.DataTypeString = dt.Item1;
                        column.DataType = dt.Item2;
                        toReturn.Add(column);
                    }
                }

                return toReturn;
            }
        }

        private Tuple<string, Type> GetDataType(string mySqlType)
        {
            switch (mySqlType.ToUpper())
            {
                case "BIT":
                case "TINYINT":
                    return new Tuple<string, Type>("byte", typeof(byte));
                case "SMALLINT":
                case "YEAR":
                    return new Tuple<string, Type>("short", typeof(short));
                case "MEDIUMINT":
                    throw new Exception("Medium Int not supported");
                case "INT":
                    return new Tuple<string, Type>("int", typeof(int));
                case "BIGINT":
                    return new Tuple<string, Type>("long", typeof(long));
                case "FLOAT":
                case "DOUBLE":
                case "DECIMAL":
                    return new Tuple<string, Type>("decimal", typeof(decimal));
                case "BOOL":
                case "BOOLEAN":
                    return new Tuple<string, Type>("bool", typeof(bool));
                case "DATE":
                case "TIME":
                case "DATETIME":
                    return new Tuple<string, Type>("DateTime", typeof(DateTime));
                case "TIMESTAMP":
                    return new Tuple<string, Type>("byte[]", typeof(byte[]));
                case "CHAR":
                case "VARCHAR":
                case "TINYTEXT":
                case "TEXT":
                case "MEDIUMTEXT":
                case "LONGTEXT":
                case "BINARY":
                case "VARBINARY":
                default:
                    return new Tuple<string, Type>("string", typeof(string));
            }
        }
    }
}