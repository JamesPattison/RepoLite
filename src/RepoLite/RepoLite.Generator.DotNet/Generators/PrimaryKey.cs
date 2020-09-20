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
    internal sealed class PrimaryKey : BaseGenerator
    {
        public PrimaryKey(
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

            //get
            sb.AppendLine(Tab2, $"{ModelName(_table.DbTableName)} Get({pk.DataTypeString} {pk.FieldName});");
            sb.AppendLine(Tab2,
                $"IEnumerable<{ModelName(_table.DbTableName)}> Get(List<{pk.DataTypeString}> {pk.FieldName}s);");
            sb.AppendLine(Tab2,
                $"IEnumerable<{ModelName(_table.DbTableName)}> Get(params {pk.DataTypeString}[] {pk.FieldName}s);");

            if (_table.PrimaryKeys.Count == 1 &&
                new[] {typeof(short), typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float)}
                    .Contains(pk.DataType))
            {
                sb.AppendLine(Tab2, $"{pk.DataTypeString} GetMaxId();");
            }

            sb.AppendLine(Tab2,
                $"bool Delete({pk.DataTypeString} {pk.FieldName});");
            sb.AppendLine(Tab2,
                $"bool Delete(IEnumerable<{pk.DataTypeString}> {pk.FieldName}s);");

            foreach (var column in _table.Columns)
            {
                sb.AppendLine(Tab2,
                    $"bool DeleteBy{column.DbColumnName}({column.DataTypeString} {column.FieldName});");
            }

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