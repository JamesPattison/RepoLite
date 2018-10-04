using System;
using System.Data;
using System.Diagnostics;

namespace RepoLite.Common.Models
{
    [DebuggerDisplay("{PropertyName} ({DataType})")]
    public class Column
    {
        public string DbColName { get; set; }
        public bool IsComputed { get; set; }
        public int SqlDataTypeCode { get; set; }
        public string SqlDataType { get; set; }
        public SqlDbType DbType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public object DefaultValue { get; set; }
        public bool PrimaryKey { get; set; }
        public int MaxLength { get; set; }
        public int MaxIntLength { get; set; }
        public int MaxDecimalLength { get; set; }



        public string PropertyName => Helpers.UpperFirst(DbColName);
        public string FieldName => Helpers.LowerFirst(DbColName);
        public Type DataType { get; set; }
    }
}
