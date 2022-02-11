using System;
using MySqlConnector;

namespace RepoLite.GeneratorEngine.Generators
{
    public class MySqlHelpers
    {
        public static MySqlDbType GetDbType(string mySqlType)
        {
            switch (mySqlType.ToLower())
            {
                case "bit": return MySqlDbType.Bit;
                case "tinyint": return MySqlDbType.Byte;
                case "smallint": return MySqlDbType.Int16;
                case "year": return MySqlDbType.Year;
                case "mediumint": throw new Exception("Medium Int not supported");
                case "int": return MySqlDbType.Int32;
                case "bigint": return MySqlDbType.Int64;
                case "float": return MySqlDbType.Float;
                case "double": return MySqlDbType.Double;
                case "decimal": return MySqlDbType.Decimal;
                case "bool": return MySqlDbType.Bit;
                case "boolean": return MySqlDbType.Bit;
                case "date": return MySqlDbType.Date;
                case "time": return MySqlDbType.Time;
                case "datetime": return MySqlDbType.DateTime;
                case "timestamp": return MySqlDbType.Timestamp;
                case "char": return MySqlDbType.VarChar;
                case "varchar": return MySqlDbType.VarChar;
                case "tinytext": return MySqlDbType.TinyText;
                case "text": return MySqlDbType.Text;
                case "mediumtext": return MySqlDbType.MediumText;
                case "longtext": return MySqlDbType.LongText;
                case "binary": return MySqlDbType.Binary;
                case "varbinary": return MySqlDbType.VarBinary;
            }
            throw new Exception("SQL Type not supported");
        }
    }
}