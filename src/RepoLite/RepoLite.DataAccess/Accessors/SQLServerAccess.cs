using Dapper;
using RepoLite.Common.Extensions;
using RepoLite.Common.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Xml;

namespace RepoLite.DataAccess.Accessors
{
    public class SQLServerAccess : DataSource<SqlConnection>
    {
        public override List<TableAndSchema> GetTables()
        {
            return GetTables("dbo");
        }

        public override List<TableAndSchema> GetTables(string schema)
        {
            using (var conn = Connection)
            {
                var tables = conn.Query<string>(@"
                    SELECT 
                        TABLE_SCHEMA + '.' + TABLE_NAME
                    FROM
                        INFORMATION_SCHEMA.TABLES
                    WHERE
                            TABLE_TYPE = 'BASE TABLE'
                        AND
                            TABLE_SCHEMA = @schema",
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
            using (var cn = Connection)
            {
                var columns = cn.Query<Column>(@"
                            SELECT
	                            c.COLUMN_NAME AS DbColName,	
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
	                            CASE WHEN x.COLUMN_NAME = c.COLUMN_NAME THEN 1 ELSE 0 END AS PrimaryKey,
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
										ccu.COLUMN_NAME
									FROM 
										INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu
									INNER JOIN 
										INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
										ON
											ccu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
											AND ccu.TABLE_SCHEMA = tc.TABLE_SCHEMA
											AND ccu.TABLE_NAME = tc.TABLE_NAME
											AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY') x
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

                foreach (var column in columns)
                {
                    column.DataType = GetDataType(column.SqlDataTypeCode);
                }

                return columns;
            }
        }

        //todo move this into a c# layer
        private Type GetDataType(int sqlType)
        {
            switch (sqlType)
            {
                case 104:   //BIT
                    return typeof(bool);
                case 48:    //TINYINT
                    return typeof(byte);
                case 34:    //IMAGE
                case 165:   //VARBINARY
                case 173:   //BINARY
                case 189:   //TIMESTAMP -- hmm
                    return typeof(byte[]);
                case 40:    //DATE
                case 42:    //DATETIME2
                case 58:    //SMALLDATETIME
                case 61:    //DATETIME
                    return typeof(DateTime);
                case 43:    //DATETIMEOFFSET
                    return typeof(DateTimeOffset);
                case 59:    //REAL
                case 60:    //MONEY
                case 62:    //FLOAT
                case 106:   //DECIMAL
                case 108:   //NUMERIC
                case 122:   //SMALLMONEY
                    return typeof(decimal);
                case 36:    //UNIQUEIDENTIFIER
                    return typeof(Guid);
                case 52:    //SMALLINT
                    return typeof(short);
                case 56:    //INT
                    return typeof(int);
                case 127:   //BIGINT
                    return typeof(long);
                case 98:    //SQL_VARIANT
                    return typeof(object);
                case 41:    //TIME
                    return typeof(TimeSpan);
                //case 240:   //GEOGRAPHY          //Not supporting
                //    return typeof(SqlGeography);
                case 241:   //XML
                    return typeof(XmlDocument);
                case 35:    //TEXT
                case 99:    //NTEXT
                case 167:   //VARCHAR
                case 175:   //CHAR
                case 231:   //NVARCHAR
                case 239:   //NCHAR
                default:
                    return typeof(string);
            }
        }

    }
}
