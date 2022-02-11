//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using MySqlConnector;

namespace Biomind.Data.Models.Base
{
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public string Error { get; set; }

        public ValidationError(string property, string error)
        {
            PropertyName = property;
            Error = error;
        }
    }
    
    public class ColumnDefinition
    {
        public string ColumnName { get; set; }
        public Type ValueType { get; set; }
        public string SqlDataTypeText { get; set; }
        public MySqlDbType MySqlDbType { get; set; }
        public bool Identity { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Nullable { get; set; }

        public ColumnDefinition(string columnName) : this(columnName, typeof(string), "text", MySqlDbType.Text, false, false, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, MySqlDbType dbType) : this(columnName, valueType, sqlDataTypeText, dbType, false, false, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, MySqlDbType dbType, bool nullable) : this(columnName, valueType, sqlDataTypeText, dbType, nullable, false, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, MySqlDbType dbType, bool nullable, bool primaryKey) : this(columnName, valueType, sqlDataTypeText, dbType, nullable, primaryKey, false) { }
        public ColumnDefinition(string columnName, Type valueType, string sqlDataTypeText, MySqlDbType dbType, bool nullable, bool primaryKey, bool identity)
        {
            ColumnName = columnName;
            ValueType = valueType;
            SqlDataTypeText = sqlDataTypeText;
            MySqlDbType = dbType;
            Nullable = nullable;
            PrimaryKey = primaryKey;
            Identity = identity;
        }
    }
    
    public class TableDefinition
    {
        public string PrimaryKey { get; set; }
        public string Schema { get; set; }
        public string TableName { get; set; }
        public IEnumerable<ColumnDefinition> Columns { get; set; }
        public TableDefinition(string primaryKey, string schema, string tableName, IEnumerable<ColumnDefinition> columns)
        {
            PrimaryKey = primaryKey;
            Schema = schema;
            TableName = tableName;
            Columns = columns;
        }
    }
    
    public partial interface IBaseModel
    {
        string EntityName { get; }
        IBaseModel SetValues(DataRow row, string propertyPrefix);
    }

    public abstract partial class BaseModel : IBaseModel
    {
        public abstract string EntityName { get; }
        public abstract IBaseModel SetValues(DataRow row, string propertyPrefix);
        public abstract List<ValidationError> Validate();
        public readonly List<string> DirtyColumns = new List<string>();

        public void ResetDirty()
        {
            DirtyColumns.Clear();
        }

        protected void SetValue<T>(ref T prop, T value, [CallerMemberName] string propName = "")
        {
            if (!DirtyColumns.Contains(propName))
            {
                DirtyColumns.Add(propName);
            }
            prop = value;
        }

        public static int GetDecimalPlaces(decimal n)
        {
            n = Math.Abs(n);
            n -= (int)n;
            var decimalPlaces = 0;
            while (n > 0)
            {
                decimalPlaces++;
                n *= 10;
                n -= (int)n;
            }
            return decimalPlaces;
        }
    }
    
    public static class Ext
    {
        public static T? GetValue<T>(this DataRow row, string columnName) where T : struct
        {
            if (row.IsNull(columnName) || !row.Table.Columns.Contains(columnName))
                return null;
    
            return row[columnName] as T?;
        }
    
        public static string GetText(this DataRow row, string columnName)
        {
            if (row.IsNull(columnName) || !row.Table.Columns.Contains(columnName))
                return null;
    
            return row[columnName] as string;
        }
    }
}