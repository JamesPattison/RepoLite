using RepoLite.Common;
using RepoLite.Common.Extensions;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static RepoLite.Common.Helpers;

namespace RepoLite.Plugins.K3
{
    public class CSharpSqlServerGeneratorImports : ICSharpSqlServerGeneratorImports
    {
        private string GetClassName(string tableClassName)
        {
            var result = Regex.Replace(
                AppSettings.Generation.ModelClassNameFormat,
                Regex.Escape("{Name}"),
                tableClassName.Replace("$", "$$"),
                RegexOptions.IgnoreCase
            );

            return result;
        }

        public StringBuilder GenerateRepoWrapper(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using Orchard.Shared;");
            sb.AppendLine("using Prolific;");
            sb.AppendLine("");
            sb.AppendLine(Tab1, $"public partial class {table.ClassName}Repository");
            sb.AppendLine(Tab1, "{");
            if (table.HasCompositeKey)
            {
                var pkParamList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.DataType.Name} {column.FieldName}, ")
                    .TrimEnd(' ', ',');
                var pkParamValsList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.FieldName}, ")
                    .TrimEnd(' ', ',');

                sb.AppendLine(Tab2, $"public static {GetClassName(table.ClassName)} Get(string profile, {pkParamList})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, $".Get({pkParamValsList});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static {GetClassName(table.ClassName)} Get(string profile, {GetClassName(table.ClassName)}Keys compositeId)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, ".Get(compositeId);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static IEnumerable<{GetClassName(table.ClassName)}> Get(string profile, List<{GetClassName(table.ClassName)}Keys> compositeIds)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, ".Get(compositeIds);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static IEnumerable<{GetClassName(table.ClassName)}> Get(string profile, params {GetClassName(table.ClassName)}Keys[] compositeIds)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, ".Get(compositeIds);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                if (table.Columns.All(x => !x.PrimaryKey))
                {
                    sb.AppendLine(Tab2, $"public static bool Update(string profile, {GetClassName(table.ClassName)} item)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                    sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                    sb.AppendLine(Tab4, $".Update(item);");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");

                    sb.AppendLine(Tab2, $"public static bool Delete(string profile, {GetClassName(table.ClassName)} item)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                    sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                    sb.AppendLine(Tab4, $".Delete(item);");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                }

                sb.AppendLine(Tab2, $"public static bool Delete(string profile, {pkParamList})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                sb.AppendLine(Tab4, $".Delete({pkParamValsList});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static bool Delete(string profile, {GetClassName(table.ClassName)}Keys compositeId)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                sb.AppendLine(Tab4, ".Delete(compositeId);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static bool Delete(string profile, IEnumerable<{GetClassName(table.ClassName)}Keys> compositeIds)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                sb.AppendLine(Tab4, ".Delete(compositeIds);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine("");
            }
            else if (table.PrimaryKeys.Any())
            {
                var pk = table.PrimaryKeys.First();
                sb.AppendLine(Tab2, $"public static {GetClassName(table.ClassName)}  Get(string profile, {pk.DataType.Name} {pk.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, $".Get({pk.FieldName});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2,
                    $"public static IEnumerable<{GetClassName(table.ClassName)} > Get(string profile, List<{pk.DataType.Name}> {pk.FieldName}s)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, $".Get({pk.FieldName}s);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2,
                    $"public static IEnumerable<{GetClassName(table.ClassName)} > Get(string profile, params {pk.DataType.Name}[] {pk.FieldName}s)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Get in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Get\"))");
                sb.AppendLine(Tab4, $".Get({pk.FieldName}s);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
                
                if (table.PrimaryKeys.Count == 1 &&
                    new[] {typeof(short), typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float)}
                        .Contains(pk.DataType))
                {
                    sb.AppendLine(Tab2, $"public static {pk.DataType.Name} GetMaxId(string profile, IEnumerable<{pk.DataType.Name}> {pk.FieldName}s)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"Log.Activity(\"Calling GetMaxId in {table.ClassName}Repository\");");
                    sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"GetMaxId\"))");
                    sb.AppendLine(Tab4, $".GetMaxId();");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                }

                sb.AppendLine(Tab2, $"public static bool Delete(string profile, {pk.DataType.Name} {pk.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                sb.AppendLine(Tab4, $".Delete({pk.FieldName});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static bool Delete(string profile, IEnumerable<{pk.DataType.Name}> {pk.FieldName}s)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                sb.AppendLine(Tab4, $".Delete({pk.FieldName}s);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine("");
            }
            else
            {
                foreach (var column in table.Columns)
                {
                    sb.AppendLine(Tab2, $"public static bool DeleteBy{column.DbColName}(string profile, {column.DataType.Name} {column.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                    sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                    sb.AppendLine(Tab4, $".DeleteBy{column.DbColName}({column.FieldName});");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                }
            }

            sb.AppendLine(Tab2, "[Obsolete(\"This method will not be removed, however it will not be updated. Please use Search or Where instead.\")]");
            sb.AppendLine(Tab2, $"public static IEnumerable<{GetClassName(table.ClassName)}> Find(string profile, ");
            var vals = "";
            foreach (var column in table.Columns)
            {
                sb.Append(Tab3,
                    $"{column.DataType.Name}{(IsNullable(column.DataType.Name) ? "?" : string.Empty)} {column.FieldName} = null");
                vals += column.FieldName;
                if (column == table.Columns.Last())
                {
                    sb.AppendLine(")");
                }
                else
                {
                    vals += ", ";
                    sb.AppendLine(", ");
                }
            }
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling Find in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Find\"))");
            sb.AppendLine(Tab4, $".Search({vals});");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");


            sb.AppendLine(Tab2, $"public static IEnumerable<{GetClassName(table.ClassName)}> Search(string profile, ");
            foreach (var column in table.Columns)
            {
                sb.Append(Tab3,
                    $"{column.DataType.Name}{(IsNullable(column.DataType.Name) ? "?" : string.Empty)} {column.FieldName} = null");
                sb.AppendLine(column == table.Columns.Last() ? ")" : ",");
            }
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling Search in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Search\"))");
            sb.AppendLine(Tab4, $".Search({vals});");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");


            if (table.HasCompositeKey)
            {
                sb.AppendLine("");
                //Find methods on PK'S are available as there's a composite primary key
                foreach (var primaryKey in table.PrimaryKeys)
                {
                    sb.AppendLine(Tab2,
                        $"public static IEnumerable<{GetClassName(table.ClassName)}> FindBy{primaryKey.PropertyName}(string profile, {primaryKey.DataType.Name} {primaryKey.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"Log.Activity(\"Calling FindBy{primaryKey.PropertyName} in {table.ClassName}Repository\");");
                    sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"FindBy{primaryKey.PropertyName}\"))");
                    sb.AppendLine(Tab4, $".FindBy{primaryKey.PropertyName}({primaryKey.FieldName});");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");

                    sb.AppendLine(Tab2,
                        $"public static IEnumerable<{GetClassName(table.ClassName)}> FindBy{primaryKey.PropertyName}(string profile, FindComparison comparison, {primaryKey.DataType.Name} {primaryKey.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"Log.Activity(\"Calling FindBy{primaryKey.PropertyName} in {table.ClassName}Repository\");");
                    sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"FindBy{primaryKey.PropertyName}\"))");
                    sb.AppendLine(Tab4, $".FindBy{primaryKey.PropertyName}(comparison, {primaryKey.FieldName});");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");

                }
            }

            sb.AppendLine("");

            foreach (var nonPrimaryKey in table.NonPrimaryKeys)
            {
                sb.AppendLine(Tab2,
                    $"public static IEnumerable<{GetClassName(table.ClassName)}> FindBy{nonPrimaryKey.PropertyName}(string profile, {nonPrimaryKey.DataType.Name} {nonPrimaryKey.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling FindBy{nonPrimaryKey.PropertyName} in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"FindBy{nonPrimaryKey.PropertyName}\"))");
                sb.AppendLine(Tab4, $".FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.FieldName});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2,
                    $"public static IEnumerable<{GetClassName(table.ClassName)}> FindBy{nonPrimaryKey.PropertyName}(string profile, FindComparison comparison, {nonPrimaryKey.DataType.Name} {nonPrimaryKey.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling FindBy{nonPrimaryKey.PropertyName} in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"FindBy{nonPrimaryKey.PropertyName}\"))");
                sb.AppendLine(Tab4, $".FindBy{nonPrimaryKey.PropertyName}(comparison, {nonPrimaryKey.FieldName});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

            }

            if (table.PrimaryKeys.Any())
            {
                sb.AppendLine(Tab2, $"public static bool Merge(string profile, List<{GetClassName(table.ClassName)}> items)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Merge in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Merge\"))");
                sb.AppendLine(Tab4, ".Merge(items);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
            }

            sb.AppendLine(Tab2, $"public static IEnumerable<{GetClassName(table.ClassName)}> GetAll(string profile)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling GetAll in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"GetAll\"))");
            sb.AppendLine(Tab4, ".GetAll();");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");

            sb.AppendLine(Tab2, $"public static bool Create(string profile, {GetClassName(table.ClassName)} item)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling Create in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Create\"))");
            sb.AppendLine(Tab4, ".Create(item);");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");

            sb.AppendLine(Tab2, $"public static bool BulkCreate(string profile, List<{GetClassName(table.ClassName)}> items)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling BulkCreate in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"BulkCreate\"))");
            sb.AppendLine(Tab4, ".BulkCreate(items);");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");

            sb.AppendLine(Tab2, $"public static bool BulkCreate(string profile, params {GetClassName(table.ClassName)}[] items)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling BulkCreate in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"BulkCreate\"))");
            sb.AppendLine(Tab4, ".BulkCreate(items);");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");

            if (table.PrimaryKeys.Any())
            {
                sb.AppendLine(Tab2, $"public static bool Update(string profile, {GetClassName(table.ClassName)} item)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Update in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Update\"))");
                sb.AppendLine(Tab4, ".Update(item);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public static bool Delete(string profile, {GetClassName(table.ClassName)} item)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"Log.Activity(\"Calling Delete in {table.ClassName}Repository\");");
                sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Delete\"))");
                sb.AppendLine(Tab4, ".Delete(item);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

            }

            sb.AppendLine(Tab2, $"public static Where<{GetClassName(table.ClassName)}> Where(string profile, string col, Comparison comparison, object val)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling Where in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Where\"))");
            sb.AppendLine(Tab4, ".Where(col, comparison, val);");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");

            sb.AppendLine(Tab2, $"public static IEnumerable<{GetClassName(table.ClassName)}> Where(string profile, string query)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"Log.Activity(\"Calling Where in {table.ClassName}Repository\");");
            sb.AppendLine(Tab3, $"return new {table.ClassName}Repository(Settings.Instance[profile].ConnectionString, exception => Log.Error(exception, \"Where\"))");
            sb.AppendLine(Tab4, ".Where(query);");
            sb.AppendLine(Tab2, "}");
            sb.AppendLine("");

            sb.AppendLine(Tab1, "}");
            return sb;
        }
    }
}
