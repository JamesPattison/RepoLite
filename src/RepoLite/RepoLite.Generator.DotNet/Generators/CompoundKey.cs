using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Options;
using RepoLite.Common.Enums;
using RepoLite.Common.Extensions;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;
using RepoLite.Generator.DotNet.Generators.Base;

namespace RepoLite.Generator.DotNet.Generators
{
    internal sealed class CompoundKey : BaseGenerator
    {
        public CompoundKey(
            IOptions<GenerationSettings> generationSettings,
            Table table) : base(generationSettings, table)
        {
        }

        public override void Interface(StringBuilder sb)
        {
            sb.AppendLine(Tab1,
                _table.Columns.Count(x => x.PrimaryKey) == 1
                    ? $"public partial interface I{RepositoryName(_table.DbTableName)} : IPkRepository<{ModelName(_table.DbTableName)}>"
                    : $"public partial interface I{RepositoryName(_table.DbTableName)} : IBaseRepository<{ModelName(_table.DbTableName)}>");

            sb.AppendLine(Tab1, "{");

            var pk = _table.PrimaryKeys.FirstOrDefault();

            var pkParamList = _table.PrimaryKeys.Aggregate("",
                    (current, column) => current + $"{column.DataTypeString} {column.FieldName}, ")
                .TrimEnd(' ', ',');

            //get
            sb.AppendLine(Tab2, $"{ModelName(_table.DbTableName)} Get({pkParamList});");
            sb.AppendLine(Tab2,
                $"{ModelName(_table.DbTableName)} Get({ModelName(_table.DbTableName)}Keys compositeId);");
            sb.AppendLine(Tab2,
                $"IEnumerable<{ModelName(_table.DbTableName)}> Get(List<{ModelName(_table.DbTableName)}Keys> compositeIds);");
            sb.AppendLine(Tab2,
                $"IEnumerable<{ModelName(_table.DbTableName)}> Get(params {ModelName(_table.DbTableName)}Keys[] compositeIds);");

            foreach (var column in _table.Columns)
            {
                sb.AppendLine(Tab2,
                    $"bool DeleteBy{column.DbColumnName}({column.DataTypeString} {column.FieldName});");
            }

            //update & delete
            sb.AppendLine(Tab2, $"bool Update({ModelName(_table.DbTableName)} item, bool clearDirty = true);");

            //delete
            sb.AppendLine(Tab2, $"bool Delete({ModelName(_table.DbTableName)} {ModelName(_table.DbTableName).ToLower()});");
            sb.AppendLine(Tab2, $"bool Delete({pkParamList});");
            sb.AppendLine(Tab2, $"bool Delete({ModelName(_table.DbTableName)}Keys compositeId);");
            sb.AppendLine(Tab2, $"bool Delete(IEnumerable<{ModelName(_table.DbTableName)}Keys> compositeIds);");

            sb.AppendLine(Tab2, $"bool Merge(List<{ModelName(_table.DbTableName)}> items);");
            sb.AppendLine(Tab2, "bool Merge(string csvPath);");

            //search
            sb.AppendLine(Tab2, $"IEnumerable<{ModelName(_table.DbTableName)}> Search(");
            foreach (var column in _table.Columns)
            {
                sb.Append(Tab3,
                    column.DataType != typeof(XmlDocument)
                        ? $"{column.DataTypeString}{(IsCSharpNullable(column.DataTypeString) ? "?" : string.Empty)} {column.FieldName} = null"
                        : $"String {column.FieldName} = null");
                if (column != _table.Columns.Last())
                    sb.AppendLine(",");
            }

            sb.AppendLine(");");

            //find
            foreach (var primaryKey in _table.PrimaryKeys)
            {
                sb.AppendLine(Tab2,
                    $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{primaryKey.PropertyName}({primaryKey.DataTypeString} {primaryKey.FieldName});");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{primaryKey.PropertyName}(FindComparison comparison, {primaryKey.DataTypeString} {primaryKey.FieldName});");
            }

            foreach (var nonPrimaryKey in _table.NonPrimaryKeys)
            {
                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName});"
                        : $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName});");

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName});"
                        : $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName});");
            }

            sb.AppendLine(Tab1, "}");
        }
    }
}
