using System;
using MySql.Data.MySqlClient;
using RepoLite.Common.Interfaces;

namespace RepoLite.GeneratorEngine
{
    public class MySqlHelpers
    {
        public static MySqlDbType GetDbType(string mySqlType)
        {
            switch (mySqlType)
            {
                case "BIT": return MySqlDbType.Bit;
                case "TINYINT": return MySqlDbType.Byte;
                case "SMALLINT": return MySqlDbType.Int16;
                case "YEAR": return MySqlDbType.Year;
                case "MEDIUMINT": throw new Exception("Medium Int not supported");
                case "INT": return MySqlDbType.Int32;
                case "BIGINT": return MySqlDbType.Int64;
                case "FLOAT": return MySqlDbType.Float;
                case "DOUBLE": return MySqlDbType.Double;
                case "DECIMAL": return MySqlDbType.Decimal;
                case "BOOL": return MySqlDbType.Bit;
                case "BOOLEAN": return MySqlDbType.Bit;
                case "DATE": return MySqlDbType.Date;
                case "TIME": return MySqlDbType.Time;
                case "DATETIME": return MySqlDbType.DateTime;
                case "TIMESTAMP": return MySqlDbType.Timestamp;
                case "CHAR": return MySqlDbType.VarChar;
                case "VARCHAR": return MySqlDbType.VarChar;
                case "TINYTEXT": return MySqlDbType.TinyText;
                case "TEXT": return MySqlDbType.Text;
                case "MEDIUMTEXT": return MySqlDbType.MediumText;
                case "LONGTEXT": return MySqlDbType.LongText;
                case "BINARY": return MySqlDbType.Binary;
                case "VARBINARY": return MySqlDbType.VarBinary;
            }
            throw new Exception("SQL Type not supported");
        }
    }
}