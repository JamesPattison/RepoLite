using Dapper;
using RepoLite.Common.Extensions;
using RepoLite.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;
using Microsoft.Extensions.Options;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models.ProcedureParameters;
using RepoLite.Common.Models.Querying;
using RepoLite.Common.Options;

namespace RepoLite.DataAccess.Accessors
{
    public class SQLServerAccess : DataSource
    {
        private SystemOptions _systemSettings;

        public SQLServerAccess(
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
            using var conn = new SqlConnection(_systemSettings.ConnectionString);
            var tables = conn.Query<string>(@"
                    SELECT 
                        TABLE_SCHEMA + '.' + TABLE_NAME
                    FROM
                        INFORMATION_SCHEMA.TABLES
                    WHERE
                            TABLE_TYPE = 'BASE TABLE'
                        AND
                            (@schema IS NULL OR TABLE_SCHEMA = @schema)",
                new {schema});

            var toReturn = tables.Select(table => table.GetTableAndSchema());
            return toReturn;
        }

        public override IEnumerable<NameAndSchema> GetProcedures()
        {
            using var cn = new SqlConnection(_systemSettings.ConnectionString);

            var procedures = cn.Query<NameAndSchema>(@"
                            SELECT
                                ROUTINE_SCHEMA  AS [Schema],
                                ROUTINE_NAME    AS Name  
                            FROM INFORMATION_SCHEMA.ROUTINES");

            return procedures;
        }

        public override IEnumerable<Procedure> LoadProcedures(IEnumerable<NameAndSchema> procedures)
        {
            foreach (var procedure in procedures)
            {
                var proc = new Procedure
                {
                    Name = procedure.Name,
                    Schema = procedure.Schema,
                    Parameters = LoadProcedureParameters(procedure).ToList()
                };

                proc.ResultSets = LoadProcedureResults(proc).ToList();
                yield return proc;
            }
        }

        private IEnumerable<IProcedureParameter> LoadProcedureParameters(NameAndSchema procedure)
        {
            using var cn = new SqlConnection(_systemSettings.ConnectionString);
            
            var parameterInfo = cn.Query<ProcedureParameterInfo>(@"
                                SELECT 
	                                pa.name				AS Name,
	                                pa.parameter_id		AS Pos,
	                                pa.system_type_id	AS SqlDataType,
	                                pa.is_output		AS IsOutput,
                                    pa.Is_Nullable		AS IsNullable,
	                                pa.max_length       AS Length,
                                    pa.[precision]      AS Precision,
                                    pa.scale            AS Scale,
	                                t.is_user_defined	AS IsUserDefined,
	                                t.is_table_type		AS IsTableType,
	                                CASE t.is_user_defined WHEN 1 THEN SCHEMA_NAME(t.schema_id) ELSE NULL END AS UserDefinedSchema,
	                                CASE t.is_user_defined WHEN 1 THEN t.name ELSE NULL END	AS UserDefinedName
                                FROM
	                                SYS.PROCEDURES p
	                                JOIN 
		                                SYS.parameters pa
			                                ON p.object_id = pa.object_id
	                                INNER JOIN 
		                                sys.types AS t 
			                                on pa.system_type_id = t.system_type_id AND pa.user_type_id = t.user_type_id
                                WHERE
		                                SCHEMA_NAME(p.schema_id) = @schema
	                                AND
		                                p.name = @name",
                new
                {
                    schema = procedure.Schema,
                    name = procedure.Name
                });

            foreach (var parameter in parameterInfo)
            {
                IProcedureParameter param;
                if (parameter.IsUserDefined)
                {
                    param = LoadCustomType(parameter);
                }
                else
                {
                    parameter.Type = GetDataType(parameter.SqlDataType).Item2;
                    param = parameter;
                }
                
                yield return param;
            }
        }

        private IEnumerable<ResultsSet> LoadProcedureResults(Procedure procedure)
        {
            using var cn = new SqlConnection(_systemSettings.ConnectionString);

            var paramString = "";
            var declares = "";
            foreach (var parameter in procedure.Parameters)
            {
                switch (parameter)
                {
                    case BasicParameter basic:
                        paramString = $"{basic.Name}={basic.Type.GetDefault()}";
                        break;
                    case TableTypeParameter table:
                        declares += $"DECLARE @tbl{table.Name} {table.SqlName};";
                        paramString += $"{table.Name}=@tbl{table.Name}";
                        break;
                }
            }
            
            var adapter = new SqlDataAdapter($@"
                SET FMTONLY ON;
                {declares}
                exec {procedure.Name} {paramString}
                SET FMTONLY OFF;
                ", cn);
            
            var customers = new DataSet();
            adapter.Fill(customers);
            
            foreach (DataTable table in customers.Tables)
            {
                var resultsSet = new ResultsSet{Values = new List<ResultsSetValue>()};
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

        private TableTypeParameter LoadCustomType(ProcedureParameterInfo parameterInfo)
        {
            using var cn = new SqlConnection(_systemSettings.ConnectionString);
            var param = new TableTypeParameter
            {
                Schema = parameterInfo.UserDefinedSchema,
                Name = parameterInfo.Name,
                SqlName = parameterInfo.UserDefinedName,
                Columns = new List<BasicParameter>()
            };
            
            var parameters = cn.Query<BasicParameter>(@"
                            SELECT 
                                c.name            AS Name,
                                c.column_id       AS Pos,
                                c.system_type_id  AS SqlDataType,
                                c.Is_Nullable     AS IsNullable,
                                c.max_length      AS Length,
                                c.[precision]     AS Precision,
                                c.scale           AS Scale
                                -- c.collation_name  AS Collation
                            FROM sys.table_types tt
                                JOIN sys.columns c
                                    ON tt.type_table_object_id = c.object_id
                            WHERE
		                            SCHEMA_NAME(tt.schema_id) = @schema
	                            AND
		                            tt.name = @name",
                new
                {
                    schema= parameterInfo.UserDefinedSchema,
                    name = parameterInfo.UserDefinedName
                }).ToList();

            foreach (var parameter in parameters)
            {
                parameter.Type = GetDataType(parameter.SqlDataType).Item2;
            }

            param.Columns.AddRange(parameters);
            
            return param;
        }

        public override IEnumerable<Column> LoadTableColumns(Table table)
        {
            using var cn = new SqlConnection(_systemSettings.ConnectionString);
            var columns = cn.Query<Column>(@"
                            SELECT
	                            c.COLUMN_NAME AS DbColumnName,	
	                            COLUMNPROPERTY(object_id('[' + c.TABLE_SCHEMA + '].[' + c.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsComputed') as IsComputed,	
								UPPER(c.DATA_TYPE) AS SqlDataType,
	                            t.system_type_id AS SqlDataTypeCode,
	                            CASE c.IS_NULLABLE
		                            WHEN 'YES' then 1
		                            ELSE 0
	                            END AS IsNullable, 
	                            COLUMNPROPERTY(object_id('[' + c.TABLE_SCHEMA + '].[' + c.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsIdentity') AS IsIdentity,
	                            REPLACE(CASE
		                            WHEN COLUMN_DEFAULT LIKE '((%' AND COLUMN_DEFAULT LIKE '%))' THEN SUBSTRING(COLUMN_DEFAULT,3,len(COLUMN_DEFAULT)-4)
		                            WHEN COLUMN_DEFAULT LIKE '(%' AND COLUMN_DEFAULT LIKE '%)' THEN SUBSTRING(COLUMN_DEFAULT,2,len(COLUMN_DEFAULT)-2)
		                            ELSE COLUMN_DEFAULT 
	                            END,'''','') AS DefaultValue,
	                            CASE WHEN x.COLUMN_NAME = c.COLUMN_NAME AND CONSTRAINT_TYPE = 'PRIMARY KEY' THEN 1 ELSE 0 END AS PrimaryKey,
	                            CASE WHEN x.COLUMN_NAME = c.COLUMN_NAME AND CONSTRAINT_TYPE = 'FOREIGN KEY' THEN 1 ELSE 0 END AS ForeignKey,
								OBJECT_NAME(x.referenced_object_id) AS ForeignKeyTargetTable,
								COL_NAME(x.referenced_object_id, x.referenced_column_id) AS ForeignKeyTargetColumn,
								ISNULL(CHARACTER_MAXIMUM_LENGTH,0) AS [MaxLength],
								ISNULL(NUMERIC_PRECISION, 0) - ISNULL(NUMERIC_SCALE, 0) AS MaxIntLength,
								ISNULL(NUMERIC_SCALE, 0) AS MaxDecimalLength
                            FROM  
		                            INFORMATION_SCHEMA.COLUMNS c
	                            INNER JOIN  
		                            sys.types t
		                            ON 
			                            c.DATA_TYPE = t.name
	                            LEFT JOIN 
									(SELECT 
										ccu.TABLE_SCHEMA,
										ccu.TABLE_NAME,
										ccu.COLUMN_NAME,
										tc.CONSTRAINT_TYPE,
										fk.referenced_object_id,
										fkc.referenced_column_id
									FROM 
										INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
									INNER JOIN 
										INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
										ON
											ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
											AND ccu.TABLE_SCHEMA = tc.TABLE_SCHEMA
											AND ccu.TABLE_NAME = tc.TABLE_NAME
									LEFT JOIN
										sys.foreign_keys fk
										ON
											tc.CONSTRAINT_NAME = fk.name
									LEFT JOIN
										sys.foreign_key_columns fkc
										ON
											fk.object_id = fkc.constraint_object_id) x
										ON
											x.TABLE_SCHEMA = c.TABLE_SCHEMA
											AND	x.TABLE_NAME = c.TABLE_NAME
											AND x.COLUMN_NAME = c.COLUMN_NAME
                            WHERE 
	                            LOWER(c.TABLE_SCHEMA + '.' + c.TABLE_NAME) = @table
								AND t.system_type_id <> 189 -- TIMESTAMP columns
                            ORDER BY 
	                            c.ORDINAL_POSITION ASC",
                new

                {
                    table = $"{table.Schema}.{table.DbTableName}".ToLower()
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
                    var dt = GetDataType(column.SqlDataTypeCode);
                    column.DataTypeString = dt.Item1;
                    column.DataType = dt.Item2;
                    toReturn.Add(column);
                }
            }

            return toReturn;
        }

        private Tuple<string, Type> GetDataType(int sqlType)
        {
            switch (sqlType)
            {
                case 104: //BIT
                    return new Tuple<string, Type>("bool", typeof(bool));
                case 48: //TINYINT
                    return new Tuple<string, Type>("byte", typeof(byte));
                case 34: //IMAGE
                case 165: //VARBINARY
                case 173: //BINARY
                case 189: //TIMESTAMP -- hmm
                    return new Tuple<string, Type>("byte[]", typeof(byte[]));
                case 40: //DATE
                case 42: //DATETIME2
                case 58: //SMALLDATETIME
                case 61: //DATETIME
                    return new Tuple<string, Type>("DateTime", typeof(DateTime));
                case 43: //DATETIMEOFFSET
                    return new Tuple<string, Type>("DateTimeOffset", typeof(DateTimeOffset));
                case 59: //REAL
                case 60: //MONEY
                case 62: //FLOAT
                case 106: //DECIMAL
                case 108: //NUMERIC
                case 122: //SMALLMONEY
                    return new Tuple<string, Type>("decimal", typeof(decimal));
                case 36: //UNIQUEIDENTIFIER
                    return new Tuple<string, Type>("Guid", typeof(Guid));
                case 52: //SMALLINT
                    return new Tuple<string, Type>("short", typeof(short));
                case 56: //INT
                    return new Tuple<string, Type>("int", typeof(int));
                case 127: //BIGINT
                    return new Tuple<string, Type>("long", typeof(long));
                case 98: //SQL_VARIANT
                    return new Tuple<string, Type>("object", typeof(object));
                case 41: //TIME
                    return new Tuple<string, Type>("TimeSpan", typeof(TimeSpan));
                //case 240:   //GEOGRAPHY          //Not supporting
                //    return typeof(SqlGeography);
                case 241: //XML
                    return new Tuple<string, Type>("XmlDocument", typeof(XmlDocument));
                case 35: //TEXT
                case 99: //NTEXT
                case 167: //VARCHAR
                case 175: //CHAR
                case 231: //NVARCHAR
                case 239: //NCHAR
                default:
                    return new Tuple<string, Type>("string", typeof(string));
            }
        }
    }
}