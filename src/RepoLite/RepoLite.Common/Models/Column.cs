using System;
using System.Data;
using System.Diagnostics;

namespace RepoLite.Common.Models
{
    [DebuggerDisplay("{DbColumnName} ({DataTypeString})")]
    public class Column
    {
        /// <summary>
        /// Populated in RepositoryGenerationObject
        /// </summary>
        public string DbTableName { get; set; }
        
        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.COLUMN_NAME
        /// </summary>
        public string DbColumnName { get; set; }

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
        /// INFORMATION_SCHEMA.TABLE_CONSTRAINTS
        /// </summary>
        public bool ForeignKey { get; set; }

        /// <summary>
        /// OBJECT_NAME(foreign_keys.referenced_object_id)
        /// </summary>
        public string ForeignKeyTargetTable { get; set; }

        /// <summary>
        /// COL_NAME(foreign_keys.referenced_object_id, foreign_key_columns.referenced_column_id) 
        /// </summary>
        public string ForeignKeyTargetColumn { get; set; }

        /// <summary>
        /// INFORMATION_SCHEMA.COLUMNS.CHARACTER_MAXIMUM_LENGTH
        /// </summary>
        public long MaxLength { get; set; }

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
        /// Mapped using SqlDataType
        /// </summary>
        public string DataTypeString { get; set; }

        /// <summary>
        /// Mapped using SqlDataType
        /// </summary>
        public Type DataType { get; set; }

        public string FieldName
        {
            get
            {
                var name = PropertyName.ToLower();
                if (Helpers.ReservedWord(name))
                    name = "_" + name;

                return name;
            }
        }

        public string PropertyName
        {
            get
            {
                var name = DbColumnName;
                if (Helpers.ReservedWord(name))
                    name = "_" + name;

                return name;
            }
        }
    }
}
