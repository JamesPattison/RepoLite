using System;
using System.Data;
using RepoLite.Common.Interfaces;

namespace RepoLite.GeneratorEngine.Generators
{
    public class SQLServerHelpers
    {
        public static SqlDbType GetDbType(string sqlType)
        {
            switch (sqlType)
            {
                case "image": return SqlDbType.Image;
                case "text": return SqlDbType.Text;
                case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
                case "date": return SqlDbType.Date;
                case "time": return SqlDbType.Time;
                case "datetime2": return SqlDbType.DateTime2;
                case "datetimeoffset": return SqlDbType.DateTimeOffset;
                case "tinyint": return SqlDbType.TinyInt;
                case "smallint": return SqlDbType.SmallInt;
                case "int": return SqlDbType.Int;
                case "smalldatetime": return SqlDbType.SmallDateTime;
                case "real": return SqlDbType.Real;
                case "money": return SqlDbType.Money;
                case "datetime": return SqlDbType.DateTime;
                case "float": return SqlDbType.Float;
                case "sql_variant": return SqlDbType.Variant;
                case "bit": return SqlDbType.Bit;
                case "decimal": return SqlDbType.Decimal;
                case "numeric": return SqlDbType.Decimal;
                case "smallmoney": return SqlDbType.SmallMoney;
                case "bigint": return SqlDbType.BigInt;
                case "varbinary": return SqlDbType.VarBinary;
                case "varchar": return SqlDbType.VarChar;
                case "binary": return SqlDbType.Binary;
                case "char": return SqlDbType.Char;
                case "timestamp": return SqlDbType.Timestamp;
                case "nvarchar": return SqlDbType.NVarChar;
                case "nchar": return SqlDbType.NChar;
                case "xml": return SqlDbType.Xml;
            }
            throw new Exception("SQL Type not supported");
        }
    }
}