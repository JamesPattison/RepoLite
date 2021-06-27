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
        
        public override List<TableAndSchema> GetTables()
        {
            return GetTables(null);
        }

        public override List<TableAndSchema> GetTables(string schema)
        {
            using (var conn = new SqlConnection(_systemSettings.ConnectionString))
            {
                var tables = conn.Query<string>(@"
                    SELECT 
                        TABLE_SCHEMA + '.' + TABLE_NAME
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

        public override List<string> GetProcedures()
        {
            throw new NotImplementedException();
        }

        public override List<Procedure> LoadProcedures(List<string> procedures)
        {
            throw new NotImplementedException();
        }

        public override List<Column> LoadTableColumns(Table table)
        {
            using (var cn = new SqlConnection(_systemSettings.ConnectionString))
            {
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
                        column.DbType = GetDbType(column.SqlDataTypeCode);
                        toReturn.Add(column);
                    }
                }

                return toReturn;
            }
        }

        //todo move this into a c# layer
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


        private SqlDbType GetDbType(int sqlType)
        {
            switch (sqlType)
            {
                case 34: return SqlDbType.Image;
                case 35: return SqlDbType.Text;
                case 36: return SqlDbType.UniqueIdentifier;
                case 40: return SqlDbType.Date;
                case 41: return SqlDbType.Time;
                case 42: return SqlDbType.DateTime2;
                case 43: return SqlDbType.DateTimeOffset;
                case 48: return SqlDbType.TinyInt;
                case 52: return SqlDbType.SmallInt;
                case 56: return SqlDbType.Int;
                case 58: return SqlDbType.SmallDateTime;
                case 59: return SqlDbType.Real;
                case 60: return SqlDbType.Money;
                case 61: return SqlDbType.DateTime;
                case 62: return SqlDbType.Float;
                case 98: return SqlDbType.Variant;
                case 104: return SqlDbType.Bit;
                case 106: return SqlDbType.Decimal;
                case 108: return SqlDbType.Decimal;
                case 122: return SqlDbType.SmallMoney;
                case 127: return SqlDbType.BigInt;
                case 165: return SqlDbType.VarBinary;
                case 167: return SqlDbType.VarChar;
                case 173: return SqlDbType.Binary;
                case 175: return SqlDbType.Char;
                case 189: return SqlDbType.Timestamp;
                case 231: return SqlDbType.NVarChar;
                case 239: return SqlDbType.NChar;
                case 241: return SqlDbType.Xml;
            }
            throw new Exception("SQL Type not supported");
        }
    }
}