using System;
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
    internal sealed class PrimaryKeyWithInheritance : BaseInheritedGenerator
    {
        private readonly Table _inheritedTable;

        public PrimaryKeyWithInheritance(
            IOptions<GenerationSettings> generationSettings,
            Table table,
            IEnumerable<Table> otherTables) : base(generationSettings, table, otherTables)
        {
        }

        public override void Interface(StringBuilder sb)
        {
            var inherits = _inheritedDependency != null;

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
                if (column.PrimaryKey || _inheritedDependency != null &&
                    column.DbColumnName == _inheritedDependency.DbColumnName) continue;

                sb.AppendLine(Tab2,
                    $"bool DeleteBy{column.DbColumnName}({column.DataTypeString} {column.FieldName});");
            }

            if (inherits)
            {
                //DeleteBy for base Tables
                DoShitRecursively(sb, _inheritedDependency, (dependency, table) =>
                {
                    foreach (var inheritedColumn in table.Columns)
                    {
                        if (inheritedColumn.PrimaryKey || dependency != null && inheritedColumn.DbColumnName == dependency.DbColumnName) continue;

                        sb.AppendLine(Tab2,
                            $"bool DeleteBy{inheritedColumn.DbColumnName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName});");
                    }
                });
            }

            //search
            sb.AppendLine(Tab2, $"IEnumerable<{ModelName(_table.DbTableName)}> Search(");
            foreach (var column in _table.Columns)
            {
                if (column.PrimaryKey || (_inheritedDependency != null &&
                                          column.DbColumnName == _inheritedDependency.DbColumnName)) continue;

                sb.Append(Tab3,
                    column.DataType != typeof(XmlDocument)
                        ? $"{column.DataTypeString}{(IsCSharpNullable(column.DataTypeString) ? "?" : string.Empty)} {column.FieldName} = null"
                        : $"String {column.FieldName} = null");
                if (column != _table.Columns.Last())
                    sb.AppendLine(",");
            }

            if (inherits)
            {
                sb.AppendLine(",");
                DoShitRecursively(sb, _inheritedDependency, (dependency, table) =>
                {
                    foreach (var inheritedColumn in table.Columns)
                    {
                        if (inheritedColumn.PrimaryKey || (dependency != null && inheritedColumn.DbColumnName == dependency.DbColumnName)) continue;

                        sb.Append(Tab3,
                            inheritedColumn.DataType != typeof(XmlDocument)
                                ? $"{inheritedColumn.DataTypeString}{(IsCSharpNullable(inheritedColumn.DataTypeString) ? "?" : string.Empty)} {inheritedColumn.FieldName} = null"
                                : $"String {inheritedColumn.FieldName} = null");
                        if (inheritedColumn != table.Columns.Last())
                            sb.AppendLine(",");
                    }
                });
            }

            sb.AppendLine(");");

            foreach (var nonPrimaryKey in _table.NonPrimaryKeys)
            {
                if (nonPrimaryKey.PrimaryKey || (_inheritedDependency != null &&
                                                 nonPrimaryKey.DbColumnName == _inheritedDependency.DbColumnName))
                    continue;

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName});"
                        : $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName});");

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName});"
                        : $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName});");
            }

            if (inherits)
            {
                //DeleteBy for base Tables
                DoShitRecursively(sb, _inheritedDependency, (dependency, table) =>
                {
                    foreach (var inheritedColumn in table.Columns)
                    {
                        if (inheritedColumn.PrimaryKey || (dependency != null && inheritedColumn.DbColumnName == dependency.DbColumnName)) continue;

                        sb.AppendLine(Tab2,
                            inheritedColumn.DataType != typeof(XmlDocument)
                                ? $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{inheritedColumn.PropertyName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName});"
                                : $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{inheritedColumn.PropertyName}(String {inheritedColumn.FieldName});");

                        sb.AppendLine(Tab2,
                            inheritedColumn.DataType != typeof(XmlDocument)
                                ? $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, {inheritedColumn.DataTypeString} {inheritedColumn.FieldName});"
                                : $"IEnumerable<{ModelName(_table.DbTableName)}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, String {inheritedColumn.FieldName});");
                    }
                });
            }

            sb.AppendLine(Tab1, "}");
        }
    }
}