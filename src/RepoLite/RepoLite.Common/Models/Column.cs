using System;
using System.Data;
using System.Diagnostics;

namespace RepoLite.Common.Models
{
    [DebuggerDisplay("{DbColumnName} ({DataType})")]
    public class Column
    {
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME
        /// </summary>
        public string DbColumnName { get; set; }
        
        /// <summary>
        /// sys.types.system_type_id
        /// </summary>
        public int SqlDataTypeCode { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.DATA_TYPE
        /// </summary>
        public string SqlDataType { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.IS_NULLABLE
        /// </summary>
        public bool IsNullable { get; set; }
        
        /// <summary>
        /// COLUMNPROPERTY('IsIdentity')
        /// </summary>
        public bool IsIdentity { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.TABLE_CONSTRAINTS
        /// </summary>
        public bool PrimaryKey { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.CHARACTER_MAXIMUM_LENGTH
        /// </summary>
        public int MaxLength { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.NUMERIC_PRECISION
        /// </summary>
        public int MaxIntLength { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.NUMERIC_SCALE
        /// </summary>
        public int MaxDecimalLength { get; set; }
        
        /// <summary>
        /// COLUMNPROPERTY('IsComputed')
        /// </summary>
        public bool IsComputed { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.COLUMN_DEFAULT
        /// </summary>
        public object DefaultValue { get; set; }
        
        /// <summary>
        /// Mapped using SqlDataTypeCode
        /// </summary>
        public SqlDbType DbType { get; set; }
        
        /// <summary>
        /// Mapped using SqlDataTypeCode
        /// </summary>
        public Type DataType { get; set; }

        public string FieldName => PropertyName.ToLower();
        public string PropertyName
        {
            get
            {
                var name = DbColumnName;
                if (Helpers.ReservedWord(name))
                    name = "@" + name;

                return name;
            }
        }
    }
}
