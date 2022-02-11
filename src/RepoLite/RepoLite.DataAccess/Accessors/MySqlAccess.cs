using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using RepoLite.Common.Extensions;
using RepoLite.Common.Models;
using RepoLite.Common.Options;

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
                    new { schema });

                var toReturn = tables.Select(table => table.GetTableAndSchema()).ToList();
                return toReturn;
            }
        }

        public override IEnumerable<NameAndSchema> GetProcedures()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ProcedureGenerationObject> LoadProcedures(IEnumerable<NameAndSchema> procedures)
        {
            throw new NotImplementedException();
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