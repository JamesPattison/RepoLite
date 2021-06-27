using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Extensions;
using RepoLite.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Options;
using RepoLite.Common.Options;
using static RepoLite.Common.Helpers;

namespace RepoLite.GeneratorEngine.Generators
{
    public class CSharpSqlServerGenerator : CodeGenerator
    {
        private GenerationOptions _generationSettings;
        private const int VARIABLE_BLOCK_SCOPE = 5;

        //private Func<string, string, string, string> GetColName = (s, table, name) => $"{(s == name ? $"nameof({table}.{name})" : $"\"{name}\"")}";

        public CSharpSqlServerGenerator(
            IOptions<GenerationOptions> generationOptions)
        {

            _generationSettings = generationOptions.Value;
        }

        public override StringBuilder ModelForTable(Table table, List<Table> otherTables)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Xml;");
            sb.AppendLine($"using {_generationSettings.ModelGenerationNamespace}.Base;");

            sb.Append(Environment.NewLine);
            sb.AppendLine(CreateModel(table, otherTables).ToString());
            return sb;
        }

        public override StringBuilder RepositoryForTable(Table table, List<Table> otherTables)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"using {_generationSettings.RepositoryGenerationNamespace}.Base;");
            sb.AppendLine($"using {_generationSettings.ModelGenerationNamespace};");
            sb.AppendLine($"using {_generationSettings.ModelGenerationNamespace}.Base;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Xml;");
            sb.Append(Environment.NewLine);

            sb.AppendLine($"namespace {_generationSettings.RepositoryGenerationNamespace}");
            sb.AppendLine("{");

            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                //build up object to use for composite searching

                sb.AppendLine(Tab1, $"public class {table.ClassName}Keys");
                sb.AppendLine(Tab1, "{");


                foreach (var column in table.PrimaryKeys)
                {
                    //Field
                    sb.AppendLine(Tab2, $"public {column.DataTypeString} {column.DbColumnName} {{ get; set; }}");
                }

                sb.AppendLine(Tab2, $"public {table.ClassName}Keys() {{}}");

                sb.AppendLine(Tab2, $"public {table.ClassName}Keys(");
                foreach (var column in table.PrimaryKeys)
                {
                    sb.Append(Tab3, $"{column.DataTypeString} {column.FieldName}");
                    sb.AppendLine(column == table.PrimaryKeys.Last() ? ")" : ",");
                }

                sb.AppendLine(Tab2, "{");
                foreach (var column in table.PrimaryKeys)
                {
                    sb.AppendLine(Tab3, $"{column.DbColumnName} = {column.FieldName};");
                }

                sb.AppendLine(Tab2, "}");

                sb.AppendLine(Tab1, "}");
            }

            //Interface
            sb.Append(Interface(table, otherTables));

            //Repo
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
            var inherits = inheritedDependency != null;

            sb.AppendLine(Tab1, $"public {(_generationSettings.GenerateSealedObjects ? "sealed " : "")}partial class {table.RepositoryName} : BaseRepository<{table.ClassName}>, I{table.RepositoryName}");

            sb.AppendLine(Tab1, "{");

            if (inherits)
            {
                sb.AppendLine(Tab2, $"private I{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat)} _{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()};\n");
            }



            sb.AppendLine(Tab2, "partial void InitializeExtension();");

            sb.AppendLine(Tab2,
                    $"public {table.RepositoryName}(string connectionString) : this(connectionString, exception => {{ }}) {{ }}");
            sb.AppendLine(Tab2,
                $"public {table.RepositoryName}(string connectionString, bool useCache, int cacheDurationInSeconds) : this(connectionString, exception => {{ }}, useCache, cacheDurationInSeconds) {{ }}");
            sb.AppendLine(Tab2,
                $"public {table.RepositoryName}(string connectionString, Action<Exception> logMethod) : this(connectionString, logMethod, false, 0) {{ }}");
            sb.AppendLine(Tab2,
                $"public {table.RepositoryName}(string connectionString, Action<Exception> logMethod, bool useCache, int cacheDurationInSeconds) : base(connectionString, logMethod,");
            sb.AppendLine(Tab3, $"{table.ClassName}.Schema, {table.ClassName}.TableName, {table.ClassName}.Columns, useCache, cacheDurationInSeconds)");

            //}
            sb.AppendLine(Tab2, "{");
            if (inherits)
            {
                sb.AppendLine(Tab3, $"_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()} = new {inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat)}(connectionString, logMethod);");
            }

            sb.AppendLine(Tab3, "InitializeExtension();");
            sb.AppendLine(Tab2, "}");

            sb.Append(Repo_ExecuteSql(table));

            sb.Append(Repo_Get(table, otherTables, inherits));

            //create
            sb.Append(Repo_Create(table, otherTables, inherits));

            //update & delete
            if (table.PrimaryKeyConfiguration != PrimaryKeyConfigurationEnum.NoKey)
            {
                sb.Append(Repo_Update(table, otherTables, inherits));
                sb.Append(Repo_Delete(table, otherTables, inherits));
            }

            //delete by
            sb.Append(Repo_DeleteBy(table, otherTables, inherits));

            //merge
            if (table.PrimaryKeyConfiguration != PrimaryKeyConfigurationEnum.NoKey)
            {
                sb.Append(Repo_Merge(table, otherTables, inheritedDependency));
            }

            //toItem
            sb.Append(Repo_ToItem(table, inheritedDependency));

            //search
            sb.Append(Repo_Search(table, otherTables, inheritedDependency));

            //find
            sb.Append(Repo_Find(table, otherTables, inherits));

            if (inherits)
            {
                sb.AppendLine(Repo_Where(table, otherTables, inherits));
            }

            sb.AppendLine(Repo_Caching(table));

            sb.AppendLine(Tab1, "}");
            sb.AppendLine("}");
            return sb;
        }

        private string Repo_ExecuteSql(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> ExecuteSql(string sql)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "return ToItems(Execute(sql), false);");
            sb.AppendLine(Tab2, "}");

            return sb.ToString();
        }

        private string Repo_Caching(Table table)
        {
            var sb = new StringBuilder();
            var pk = table.PrimaryKeys.First();

            sb.AppendLine(Tab2, $"private void SaveToCache({table.ClassName} {table.LowerClassName})");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"CacheHelper.SaveToCache({table.ClassName}.CacheKey({table.LowerClassName}.{pk.PropertyName}), {table.LowerClassName});");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"private {table.ClassName} GetFromCache({pk.DataTypeString} {pk.FieldName})");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"return CacheHelper.GetFromCache<{table.ClassName}>({table.ClassName}.CacheKey({pk.FieldName}));");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"private void RemoveFromCache({pk.DataTypeString} {pk.FieldName})");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"CacheHelper.RemoveFromCache({table.ClassName}.CacheKey({pk.FieldName}));");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"private bool IsInCache({pk.DataTypeString} {pk.FieldName})");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"return CacheHelper.IsInCache({table.ClassName}.CacheKey({pk.FieldName}));");
            sb.AppendLine(Tab2, "}");

            return sb.ToString();
        }

        private string Repo_Where(Table table, List<Table> otherTables, bool inherits)
        {
            var sb = new StringBuilder();

            sb.AppendLine(Tab2, $"public override Where<{table.ClassName}> Where(string col, Comparison comparison, object val, Type valueType)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var tables = new List<TableDefinition>");
            sb.AppendLine(Tab3, "{");
            foreach (var definition in GetTableDefinitions(table, otherTables))
            {
                sb.AppendLine(Tab4, $"new TableDefinition(\"{definition.PrimaryKeys[0].DbColumnName}\", {definition.ClassName}.Schema, {definition.ClassName}.TableName, {definition.ClassName}.Columns),");
            }
            sb.AppendLine(Tab3, "};");
            sb.AppendLine(Tab3, "return base.Where(tables, col, comparison, val, valueType);");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"public override Where<{table.ClassName}> Where(string col, Comparison comparison, object val)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var tables = new List<TableDefinition>");
            sb.AppendLine(Tab3, "{");
            foreach (var definition in GetTableDefinitions(table, otherTables))
            {
                sb.AppendLine(Tab4, $"new TableDefinition(\"{definition.PrimaryKeys[0].DbColumnName}\", {definition.ClassName}.Schema, {definition.ClassName}.TableName, {definition.ClassName}.Columns),");
            }
            sb.AppendLine(Tab3, "};");
            sb.AppendLine(Tab3, "return base.Where(tables, col, comparison, val, val.GetType());");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"public override IEnumerable<{table.ClassName}> Where(string query)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var tables = new List<TableDefinition>");
            sb.AppendLine(Tab3, "{");
            foreach (var definition in GetTableDefinitions(table, otherTables))
            {
                sb.AppendLine(Tab4, $"new TableDefinition(\"{definition.PrimaryKeys[0].DbColumnName}\", {definition.ClassName}.Schema, {definition.ClassName}.TableName, {definition.ClassName}.Columns),");
            }
            sb.AppendLine(Tab3, "};");
            sb.AppendLine(Tab3, "return base.Where(tables, query);");
            sb.AppendLine(Tab2, "}");

            return sb.ToString();
        }

        private IEnumerable<Table> GetTableDefinitions(Table table, List<Table> otherTables)
        {
            yield return table;

            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
            if (inheritedDependency == null) yield break;

            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            foreach (var tableDefinition in GetTableDefinitions(inheritedTable, otherTables))
            {
                yield return tableDefinition;
            }

        }

        public override string FileExtension()
        {
            return "cs";
        }

        #region Helpers Methods

        private StringBuilder CreateModel(Table table, List<Table> otherTables)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"namespace {_generationSettings.ModelGenerationNamespace}");
            sb.AppendLine("{");

            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
            var inherits = inheritedDependency != null;
            if (inheritedDependency != null)
            {
                //This table inherits from another table, inherit that table instead
                sb.AppendLine(Tab1, $"public {(_generationSettings.GenerateSealedObjects ? "sealed " : "")}partial class {table.ClassName} : {inheritedDependency.ForeignKeyTargetTable.ToModelName(_generationSettings.ModelClassNameFormat)}");
            }
            else
            {
                sb.AppendLine(Tab1, $"public {(_generationSettings.GenerateSealedObjects ? "sealed " : "")}partial class {table.ClassName} : BaseModel");
            }

            sb.AppendLine(Tab1, "{");

            sb.AppendLine(Tab2, $"public override string EntityName => \"{table.DbTableName}\";");
            if (table.PrimaryKeyConfiguration != PrimaryKeyConfigurationEnum.NoKey)
            {
                sb.AppendLine(Tab2,
                    $"public static string CacheKey({table.PrimaryKeys[0].DataTypeString} id) => $\"{table.DbTableName}_{{id}}\";");
            }

            foreach (var column in table.Columns)
            {
                if (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;
                //Field
                sb.AppendLine(Tab2, $"protected {column.DataTypeString}{(IsCSharpNullable(column.DataTypeString) && column.IsNullable ? "?" : "")} _{column.FieldName};");
            }

            sb.Append(Environment.NewLine);
            foreach (var column in table.Columns)
            {
                if (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;

                //Property
                var fieldName = $"_{column.FieldName}";
                sb.AppendLine(Tab2,
                    $"public {column.DataTypeString}{(IsCSharpNullable(column.DataTypeString) && column.IsNullable ? "?" : "")} {column.DbColumnName}");
                sb.AppendLine(Tab2, "{");

                sb.AppendLine(Tab3, $"get => {fieldName};");
                sb.AppendLine(Tab3, $"set => SetValue(ref {fieldName}, value);");


                sb.AppendLine(Tab2, "}");

                if (column.ForeignKey)
                {
                    sb.AppendLine(Tab2, "/// <summary>");
                    sb.AppendLine(Tab2, "/// Nothing is done with this, it's merely there to hold data IF you wish to populate it");
                    sb.AppendLine(Tab2, "/// </summary>");
                    sb.AppendLine(Tab2,
                        $"public {column.ForeignKeyTargetTable.ToModelName(_generationSettings.ModelClassNameFormat)} {column.DbColumnName.TrimEnd('d', 'I')} {{ get; set; }}");
                }
            }

            //constructors
            sb.AppendLine(Tab2, $"public {table.ClassName}() {{ }}");

            sb.AppendLine(Tab2, $"public {table.ClassName}(");
            foreach (var column in table.Columns)
            {
                if (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;
                sb.Append(Tab3,
                        $"{column.DataTypeString} {column.FieldName}");
                sb.AppendLine(column == table.Columns.Last() && !inherits ? ")" : ",");
            }

            if (inherits)
            {
                BuildConstructorInheritedColumns(otherTables, inheritedDependency, sb);
            }

            sb.AppendLine(Tab2, "{");
            foreach (var column in table.Columns)
            {
                if (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;
                sb.AppendLine(Tab3, $"_{column.FieldName} = {column.FieldName};");
            }

            if (inherits)
            {
                BuildConstructorInheritedColumnAssignments(otherTables, inheritedDependency, sb);
            }

            sb.AppendLine(Tab2, "}");

            //methods

            sb.AppendLine(Tab2, "public override IBaseModel SetValues(DataRow row, string propertyPrefix)");
            sb.AppendLine(Tab2, "{");
            if (inheritedDependency != null)
            {
                sb.AppendLine(Tab3, $"base.SetValues(row, propertyPrefix);");
            }
            foreach (var column in table.Columns)
            {
                if (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;

                if (column.DataType == typeof(string))
                {
                    sb.AppendLine(Tab3, $"_{column.FieldName} = row.GetText($\"{{propertyPrefix}}{column.PropertyName}\");");
                }
                else if (column.DataType == typeof(byte))
                {
                    sb.AppendLine(Tab3, $"_{column.FieldName} = (byte)row[$\"{{propertyPrefix}}{column.PropertyName}\"];");
                }
                else if (column.DataType == typeof(byte?))
                {
                    sb.AppendLine(Tab3, $"_{column.FieldName} = (byte?)row[$\"{{propertyPrefix}}{column.PropertyName}\"];");
                }
                else if (column.DataType == typeof(byte[]))
                {
                    sb.AppendLine(Tab3, $"_{column.FieldName} = (byte[])row[$\"{{propertyPrefix}}{column.PropertyName}\"];");
                }
                else if (column.DataType == typeof(XmlDocument))
                {
                    sb.AppendLine(Tab3, $"_{column.FieldName} = new XmlDocument{{InnerXml = row.GetText($\"{{propertyPrefix}}{column.PropertyName}\")}};");
                }
                else
                {
                    sb.AppendLine(Tab3, $"_{column.FieldName} = row.GetValue<{column.DataTypeString}>($\"{{propertyPrefix}}{column.PropertyName}\"){(IsCSharpNullable(column.DataTypeString) && column.IsNullable ? ";" : $" ?? default({column.DataTypeString});")} ");
                }
            }

            sb.AppendLine(Tab3, "return this;");
            sb.AppendLine(Tab2, "}");

            CreateModelValidation(table, sb, inherits);

            sb.AppendLine(Tab2, $"public static string Schema = \"{table.Schema}\";");
            sb.AppendLine(Tab2, $"public static string TableName = \"{table.DbTableName}\";");
            sb.AppendLine(Tab2, "public static List<ColumnDefinition> Columns => new List<ColumnDefinition>");
            sb.AppendLine(Tab2, "{");
            foreach (var column in table.Columns)
            {
                var sqlPrecisionColumns = new[] { 35, 60, 62, 99, 106, 108, 122, 165, 167, 173, 175, 231, 239 };
                var colLengthVal = sqlPrecisionColumns.Contains(column.SqlDataTypeCode)
                    ? $"({Math.Max(column.MaxLength, column.MaxIntLength)})"
                    : string.Empty;

                if (column.DbType == SqlDbType.NVarChar && column.MaxLength == -1)
                {
                    colLengthVal = "(MAX)";
                }
                sb.AppendLine(Tab3,
                    $"new ColumnDefinition({(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")}, " +
                    $"typeof({column.DataTypeString}), " +
                    $"\"[{column.SqlDataType}]{colLengthVal}\", " +
                    $"SqlDbType.{column.DbType}, " +
                    $"{column.IsNullable.ToString().ToLower()}, " +
                    $"{column.PrimaryKey.ToString().ToLower()}, " +
                    $"{column.IsIdentity.ToString().ToLower()}),");
            }
            sb.AppendLine(Tab2, "};");

            sb.AppendLine(Tab1, "}");
            sb.AppendLine("}");

            return sb;
        }

        private static void BuildConstructorInheritedColumns(List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && x.ForeignKeyTargetTable != inheritedTable.DbTableName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedTableInheritedDependency != null &&
                        inheritedColumn.DbColumnName == inheritedTableInheritedDependency.DbColumnName) continue;
                    sb.Append(Tab3,
                        $"{inheritedColumn.DataTypeString} {inheritedColumn.FieldName}");
                    sb.AppendLine(inheritedColumn == inheritedTable.Columns.Last() && !inheritedTableAlsoInherits ? ")" : ",");
                }

                if (inheritedTableAlsoInherits)
                {
                    BuildConstructorInheritedColumns(otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private void BuildConstructorInheritedColumnAssignments(List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedTableInheritedDependency != null && inheritedColumn.DbColumnName == inheritedTableInheritedDependency.DbColumnName) continue;
                    sb.AppendLine(Tab3, $"_{inheritedColumn.FieldName} = {inheritedColumn.FieldName};");
                }

                if (inheritedTableAlsoInherits)
                {
                    BuildConstructorInheritedColumnAssignments(otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private void CreateModelValidation(Table table, StringBuilder sb, bool inherits)
        {
            sb.AppendLine(Tab2, "public override List<ValidationError> Validate()");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var validationErrors = new List<ValidationError>();");

            if (inherits)
            {
                sb.AppendLine(Tab3, $"validationErrors.AddRange(base.Validate());");
            }
            sb.AppendLine("");

            foreach (var column in table.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    if (!column.IsNullable)
                    {
                        sb.AppendLine(Tab3, $"if ({column.DbColumnName} == null)");
                        sb.AppendLine(Tab4,
                            $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot be null\"));");
                    }

                    if (column.MaxLength > 0)
                    {
                        sb.AppendLine(Tab3,
                            $"if (!string.IsNullOrEmpty({column.DbColumnName}) && {column.DbColumnName}.Length > {column.MaxLength})");
                        sb.AppendLine(Tab4,
                            $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Max length is {column.MaxLength}\"));");
                    }
                }
                else if (column.DataType == typeof(byte[]))
                {
                    if (!column.IsNullable)
                    {
                        sb.AppendLine(Tab3, $"if ({column.DbColumnName} == null)");
                        sb.AppendLine(Tab4,
                            $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot be null\"));");
                    }

                    if (column.MaxLength > 0)
                    {
                        sb.AppendLine(Tab3,
                            $"if ({column.DbColumnName} != null && {column.DbColumnName}.Length > {column.MaxLength})");
                        sb.AppendLine(Tab4,
                            $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Binary array values exceed database size\"));");
                    }
                }
                else
                    switch (Activator.CreateInstance(column.DataType))
                    {
                        case decimal _:
                            long maxValueForNum = 0;
                            for (var i = 0; i < column.MaxIntLength; i++)
                            {
                                maxValueForNum *= 10;
                                maxValueForNum += 9;
                            }

                            sb.AppendLine(Tab3,
                                $"if ({(column.IsNullable ? $"{column.DbColumnName}.HasValue && " : "")}Math.Floor({column.DbColumnName}{(column.IsNullable ? ".Value" : "")}) > {maxValueForNum})");

                            sb.AppendLine(Tab4,
                                $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot exceed {maxValueForNum}\"));");
                            sb.AppendLine(Tab3,
                                $"if ({(column.IsNullable ? $"{column.DbColumnName}.HasValue && " : "")}GetDecimalPlaces({column.DbColumnName}{(column.IsNullable ? ".Value" : "")}) > {column.MaxDecimalLength})");

                            sb.AppendLine(Tab4,
                                $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot have more than {column.MaxDecimalLength} decimal place{(column.MaxDecimalLength > 1 ? "s" : "")}\"));");
                            break;
                        case DateTime _:
                            sb.AppendLine(Tab3, $"if ({column.PropertyName} == DateTime.MinValue)");

                            sb.AppendLine(Tab4,
                                $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot be default.\"));");
                            break;
                        case TimeSpan _:
                            sb.AppendLine(Tab3, $"if ({column.DbColumnName} == TimeSpan.MinValue)");

                            sb.AppendLine(Tab4,
                                $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot be default.\"));");
                            break;
                        case DateTimeOffset _:
                            sb.AppendLine(Tab3, $"if ({column.DbColumnName} == DateTimeOffset.MinValue)");

                            sb.AppendLine(Tab4,
                                $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot be default.\"));");
                            break;
                        case Guid _:
                            sb.AppendLine(Tab3, $"if ({column.DbColumnName} == Guid.Empty)");

                            sb.AppendLine(Tab4,
                                $"validationErrors.Add(new ValidationError(nameof({column.DbColumnName}), \"Value cannot be default.\"));");
                            break;
                    }
            }

            sb.AppendLine("");
            sb.AppendLine(Tab3, "return validationErrors;");
            sb.AppendLine(Tab2, "}");
        }

        private StringBuilder PrintBlockScopedVariables(List<Column> columns)
        {
            var sb = new StringBuilder();
            var distinctColumns = columns.ToList();

            for (var i = 0; i < distinctColumns.Count; i += VARIABLE_BLOCK_SCOPE)
            {
                for (var j = 0; j < Math.Min(VARIABLE_BLOCK_SCOPE, distinctColumns.Count - i); j++)
                {
                    sb.Append($"item.{distinctColumns[i + j].PropertyName}");
                    if (distinctColumns[i + j] != distinctColumns.Last()) sb.Append(", ");
                }

                if (i + VARIABLE_BLOCK_SCOPE >= distinctColumns.Count)
                    continue;
                sb.AppendLine("");
                sb.Append(Tab4);
            }

            return sb;
        }

        #region Repo Generation 

        private StringBuilder Interface(Table table, List<Table> otherTables)
        {
            var sb = new StringBuilder();

            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
            var inherits = inheritedDependency != null;

            sb.AppendLine(Tab1,
                table.Columns.Count(x => x.PrimaryKey) == 1
                    ? $"public partial interface I{table.RepositoryName} : IPkRepository<{table.ClassName}>"
                    : $"public partial interface I{table.RepositoryName} : IBaseRepository<{table.ClassName}>");

            sb.AppendLine(Tab1, "{");

            var pk = table.PrimaryKeys.FirstOrDefault();

            var pkParamList = table.PrimaryKeys.Aggregate("",
                    (current, column) => current + $"{column.DataTypeString} {column.FieldName}, ")
                .TrimEnd(' ', ',');

            sb.AppendLine(Tab2, $"IEnumerable<{table.ClassName}> ExecuteSql(string sql);");

            //get
            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                sb.AppendLine(Tab2, $"{table.ClassName} Get({pkParamList});");
                sb.AppendLine(Tab2,
                    $"{table.ClassName} Get({table.ClassName}Keys compositeId);");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(List<{table.ClassName}Keys> compositeIds);");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(params {table.ClassName}Keys[] compositeIds);");
            }
            else if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.PrimaryKey)
            {
                sb.AppendLine(Tab2, $"{table.ClassName} Get({pk.DataTypeString} {pk.FieldName});");
                sb.AppendLine(Tab2, $"{table.ClassName} Get({pk.DataTypeString} {pk.FieldName}, bool skipCache);");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(List<{pk.DataTypeString}> {pk.FieldName}s);");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(List<{pk.DataTypeString}> {pk.FieldName}s, bool skipCache);");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(params {pk.DataTypeString}[] {pk.FieldName}s);");

                if (table.PrimaryKeys.Count == 1 &&
                    new[] { typeof(short), typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float) }
                        .Contains(pk.DataType))
                {
                    sb.AppendLine(Tab2, $"{pk.DataTypeString} GetMaxId();");
                }

                sb.AppendLine(Tab2,
                    $"bool Delete({pk.DataTypeString} {pk.FieldName});");
                sb.AppendLine(Tab2,
                    $"bool Delete(IEnumerable<{pk.DataTypeString}> {pk.FieldName}s);");
            }

            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey || inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;

                sb.AppendLine(Tab2,
                    $"bool DeleteBy{column.DbColumnName}({column.DataTypeString} {column.FieldName});");
            }

            if (inherits)
            {
                //DeleteBy for base Tables
                AppendDeleteByForInherited(otherTables, inheritedDependency, sb);
            }

            //update & delete
            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                //update
                sb.AppendLine(Tab2, $"bool Update({table.ClassName} item);");

                //delete
                sb.AppendLine(Tab2, $"bool Delete({table.ClassName} {table.LowerClassName});");
                sb.AppendLine(Tab2, $"bool Delete({pkParamList});");
                sb.AppendLine(Tab2, $"bool Delete({table.ClassName}Keys compositeId);");
                sb.AppendLine(Tab2, $"bool Delete(IEnumerable<{table.ClassName}Keys> compositeIds);");

                sb.AppendLine(Tab2, $"bool Merge(List<{table.ClassName}> items);");
                sb.AppendLine(Tab2, "bool Merge(string csvPath);");
            }

            //search
            sb.AppendLine(Tab2, $"IEnumerable<{table.ClassName}> Search(");
            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey || (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName)) continue;

                sb.Append(Tab3,
                    column.DataType != typeof(XmlDocument)
                        ? $"{column.DataTypeString}{(IsCSharpNullable(column.DataTypeString) ? "?" : string.Empty)} {column.FieldName} = null"
                        : $"String {column.FieldName} = null");
                if (column != table.Columns.Last())
                    sb.AppendLine(",");
            }

            if (inherits)
            {
                AppendSearchByForInherited(otherTables, inheritedDependency, sb);
            }

            sb.AppendLine(");");

            //find
            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                foreach (var primaryKey in table.PrimaryKeys)
                {
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{primaryKey.PropertyName}({primaryKey.DataTypeString} {primaryKey.FieldName});");
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{primaryKey.PropertyName}(FindComparison comparison, {primaryKey.DataTypeString} {primaryKey.FieldName});");
                }
            }

            foreach (var nonPrimaryKey in table.NonPrimaryKeys)
            {
                if (nonPrimaryKey.PrimaryKey || (inheritedDependency != null && nonPrimaryKey.DbColumnName == inheritedDependency.DbColumnName)) continue;

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName});"
                        : $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName});");

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName}, bool skipCache);"
                        : $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName}, bool skipCache);");

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName});"
                        : $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName});");

                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName}, bool skipCache);"
                        : $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName}, bool skipCache);");
            }

            if (inherits)
            {
                //DeleteBy for base Tables
                AppendFindByForInherited(table.ClassName, otherTables, inheritedDependency, sb);
            }

            sb.AppendLine(Tab1, "}");
            return sb;
        }

        private void AppendDeleteByForInherited(List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedColumn.PrimaryKey || inheritedDependency != null && inheritedColumn.DbColumnName == inheritedDependency.DbColumnName) continue;

                    sb.AppendLine(Tab2,
                        $"bool DeleteBy{inheritedColumn.DbColumnName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName});");
                }

                if (inheritedTableAlsoInherits)
                {
                    AppendDeleteByForInherited(otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private void AppendSearchByForInherited(List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            sb.AppendLine(",");
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedColumn.PrimaryKey || (inheritedDependency != null && inheritedColumn.DbColumnName == inheritedDependency.DbColumnName)) continue;

                    sb.Append(Tab3,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"{inheritedColumn.DataTypeString}{(IsCSharpNullable(inheritedColumn.DataTypeString) ? "?" : string.Empty)} {inheritedColumn.FieldName} = null"
                            : $"String {inheritedColumn.FieldName} = null");
                    if (inheritedColumn != inheritedTable.Columns.Last())
                        sb.AppendLine(",");
                }

                if (inheritedTableAlsoInherits)
                {
                    AppendSearchByForInherited(otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private void AppendFindByForInherited(string className, List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedColumn.PrimaryKey || (inheritedDependency != null && inheritedColumn.DbColumnName == inheritedDependency.DbColumnName)) continue;

                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName});"
                            : $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(String {inheritedColumn.FieldName});");

                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName}, bool skipCache);"
                            : $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(String {inheritedColumn.FieldName}, bool skipCache);");

                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, {inheritedColumn.DataTypeString} {inheritedColumn.FieldName});"
                            : $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, String {inheritedColumn.FieldName});");

                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, {inheritedColumn.DataTypeString} {inheritedColumn.FieldName}, bool skipCache);"
                            : $"IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, String {inheritedColumn.FieldName}, bool skipCache);");
                }

                if (inheritedTableAlsoInherits)
                {
                    AppendFindByForInherited(className, otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private StringBuilder Repo_Get(Table table, List<Table> otherTables, bool inherits)
        {
            var sb = new StringBuilder();
            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                var pkParamList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.DataTypeString} {column.FieldName}, ")
                    .TrimEnd(' ', ',');

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public {table.ClassName} Get({pkParamList})");
                sb.AppendLine(Tab2, "{");
                sb.Append(Tab3, "return Where(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append(
                        $"{(pk.DbColumnName == nameof(pk.DbColumnName) ? $"nameof({table.ClassName}.{pk.DbColumnName})" : $"\"{pk.DbColumnName}\"")}, Comparison.Equals, {pk.FieldName}");
                    if (pk != table.PrimaryKeys.Last())
                    {
                        sb.Append(").And(");
                    }
                }

                sb.AppendLine(").Results().FirstOrDefault();");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    $"public {table.ClassName} Get({table.ClassName}Keys compositeId)");
                sb.AppendLine(Tab2, "{");
                sb.Append(Tab3, "return Where(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append(
                        $"{(pk.DbColumnName == nameof(pk.DbColumnName) ? $"nameof({table.ClassName}.{pk.DbColumnName})" : $"\"{pk.DbColumnName}\"")}, Comparison.Equals, compositeId.{pk.PropertyName}");
                    if (pk != table.PrimaryKeys.Last())
                    {
                        sb.Append(").And(");
                    }
                }

                sb.AppendLine(").Results().FirstOrDefault();");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    $"public IEnumerable<{table.ClassName}> Get(List<{table.ClassName}Keys> compositeIds)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, "return Get(compositeIds.ToArray());");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    $"public IEnumerable<{table.ClassName}> Get(params {table.ClassName}Keys[] compositeIds)");
                sb.AppendLine(Tab2, "{");

                sb.Append(Tab3, "var result = Where(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append(
                        $"{(pk.DbColumnName == nameof(pk.DbColumnName) ? $"nameof({table.ClassName}.{pk.DbColumnName})" : $"\"{pk.DbColumnName}\"")}, Comparison.In, compositeIds.Select(x => x.{pk.PropertyName}).ToList()");
                    if (pk != table.PrimaryKeys.Last())
                    {
                        sb.Append(").Or(");
                    }
                }

                sb.AppendLine(").Results().ToArray();");

                sb.AppendLine(Tab3, $"var filteredResults = new List<{table.ClassName}>();");
                sb.AppendLine("");

                sb.AppendLine(Tab3, "foreach (var compositeKey in compositeIds)");
                sb.AppendLine(Tab3, "{");
                sb.Append(Tab4,
                    "filteredResults.AddRange(result.Where(x => ");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append($"x.{pk.DbColumnName} == compositeKey.{pk.DbColumnName}");
                    if (pk != table.PrimaryKeys.Last())
                        sb.Append(" && ");
                }

                sb.AppendLine("));");
                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab3, "return filteredResults;");

                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
            }
            else if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.PrimaryKey)
            {
                var pk = table.PrimaryKeys.First();

                if (inherits)
                {
                    sb.AppendLine("");
                    sb.AppendLine(Tab2, $"public {table.ClassName} Get({pk.DataTypeString} {pk.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"return Get({pk.FieldName}, false);");
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2, $"public {table.ClassName} Get({pk.DataTypeString} {pk.FieldName}, bool skipCache)");
                    sb.AppendLine(Tab2, "{");

                    {
                        sb.AppendLine(Tab3, $"if (CacheEnabled && !skipCache)");
                        sb.AppendLine(Tab3, "{");
                        sb.AppendLine(Tab4, $"var fromCache = GetFromCache({pk.FieldName});");
                        sb.AppendLine(Tab4, $"if (fromCache != null)");
                        sb.AppendLine(Tab5, $"return fromCache;");
                        sb.AppendLine(Tab3, "}");


                        sb.AppendLine(Tab3, "var query = $@\"SELECT * FROM ");
                        sb.AppendLine(Tab8, $"[{table.DbTableName}] {table.DbTableName.ToLower()[0]}");
                        var inheritedTable = table;
                        var originalAlias = table.DbTableName.ToLower()[0];
                        var previousAlias = table.DbTableName.ToLower()[0];
                        var inheritedDependency =
                            inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
                        do
                        {
                            sb.AppendLine(Tab7, $"LEFT JOIN [{inheritedDependency.ForeignKeyTargetTable}] {inheritedDependency.ForeignKeyTargetTable.ToLower()[0]}");
                            sb.AppendLine(Tab8,
                                $"ON {previousAlias}.{pk.DbColumnName} = {inheritedDependency.ForeignKeyTargetTable.ToLower()[0]}.{pk.DbColumnName}");

                            previousAlias = inheritedDependency.ForeignKeyTargetTable.ToLower()[0];
                            inheritedTable = otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
                            inheritedDependency = inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
                        } while (inheritedDependency != null);

                        sb.AppendLine(Tab7, " WHERE");
                        sb.AppendLine(Tab8, $" {originalAlias}.{pk.DbColumnName} = {{{pk.FieldName}}}\";");
                        sb.AppendLine(Tab3, "var item = ExecuteSql(query).FirstOrDefault(); ");


                        sb.AppendLine(Tab3, "if (CacheEnabled & !skipCache)");
                        sb.AppendLine(Tab3, "{");
                        sb.AppendLine(Tab4, "SaveToCache(item);");
                        sb.AppendLine(Tab3, "}");

                        sb.AppendLine(Tab3, "return item;");
                    }
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> Get(List<{pk.DataTypeString}> {pk.FieldName}s)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"return Get({pk.FieldName}s, false);");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> Get(List<{pk.DataTypeString}> {pk.FieldName}s, bool skipCache)");
                    sb.AppendLine(Tab2, "{");
                    {
                        sb.AppendLine(Tab3, $"var toReturn = new List<{table.ClassName}>();");
                        sb.AppendLine(Tab3, $"if (!{pk.FieldName}s.Any()) return toReturn;");
                        sb.AppendLine(Tab3, "if (CacheEnabled && !skipCache)");
                        sb.AppendLine(Tab3, "{");
                        sb.AppendLine(Tab4, $"var cachedIds = new List<{pk.DataTypeString}>();");
                        sb.AppendLine(Tab4, $"foreach (var {pk.FieldName} in {pk.FieldName}s.Where(IsInCache))");
                        sb.AppendLine(Tab4, "{");
                        sb.AppendLine(Tab5, $"cachedIds.Add({pk.FieldName});");
                        sb.AppendLine(Tab5, $"toReturn.Add(GetFromCache({pk.FieldName}));");
                        sb.AppendLine(Tab4, "}");
                        sb.AppendLine(Tab4, $"{pk.FieldName}s = {pk.FieldName}s.Except(cachedIds).ToList();");
                        sb.AppendLine(Tab4, $"if (!{pk.FieldName}s.Any()) return toReturn;");
                        sb.AppendLine(Tab3, "}");

                        sb.AppendLine(Tab3, "var query = $@\"SELECT * FROM ");
                        sb.AppendLine(Tab8, $"[{table.DbTableName}] {table.DbTableName.ToLower()[0]}");
                        var inheritedTable = table;
                        var originalAlias = table.DbTableName.ToLower()[0];
                        var previousAlias = table.DbTableName.ToLower()[0];
                        var inheritedDependency =
                            inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
                        do
                        {
                            sb.AppendLine(Tab7, $"LEFT JOIN [{inheritedDependency.ForeignKeyTargetTable}] {inheritedDependency.ForeignKeyTargetTable.ToLower()[0]}");
                            sb.AppendLine(Tab8,
                                $"ON {previousAlias}.{pk.DbColumnName} = {inheritedDependency.ForeignKeyTargetTable.ToLower()[0]}.{pk.DbColumnName}");

                            previousAlias = inheritedDependency.ForeignKeyTargetTable.ToLower()[0];
                            inheritedTable = otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
                            inheritedDependency = inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
                        } while (inheritedDependency != null);

                        sb.AppendLine(Tab7, " WHERE");
                        sb.AppendLine(Tab8, $" {originalAlias}.{pk.DbColumnName} IN ({{string.Join(\", \", {pk.FieldName}s)}})\";");



                        sb.AppendLine(Tab3, "var items = ExecuteSql(query).ToArray();");
                        sb.AppendLine(Tab3, "if (CacheEnabled && !skipCache)");
                        sb.AppendLine(Tab3, "{");
                        sb.AppendLine(Tab4, "foreach (var item in items)");
                        sb.AppendLine(Tab4, "{");
                        sb.AppendLine(Tab5, "SaveToCache(item);");
                        sb.AppendLine(Tab4, "}");
                        sb.AppendLine(Tab3, "}");
                        sb.AppendLine(Tab3, "toReturn.AddRange(items);");
                        sb.AppendLine(Tab3, "return toReturn;");
                    }
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> Get(params {pk.DataTypeString}[] {pk.FieldName}s)");
                    sb.AppendLine(Tab2, "{");

                    sb.AppendLine(Tab3, $"return Get({pk.FieldName}s.ToList());");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                }
                else
                {
                    sb.AppendLine("");
                    sb.AppendLine(Tab2, $"public {table.ClassName} Get({pk.DataTypeString} {pk.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"return Get({pk.FieldName}, false);");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                    sb.AppendLine(Tab2, $"public {table.ClassName} Get({pk.DataTypeString} {pk.FieldName}, bool skipCache)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"if (CacheEnabled && !skipCache)");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"var fromCache = GetFromCache({pk.FieldName});");
                    sb.AppendLine(Tab4, $"if (fromCache != null)");
                    sb.AppendLine(Tab5, $"return fromCache;");
                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab3,
                        $"return Where({(pk.DbColumnName == nameof(pk.DbColumnName) ? $"nameof({table.ClassName}.{pk.DbColumnName})" : $"\"{pk.DbColumnName}\"")}, Comparison.Equals, {pk.FieldName}).Results().FirstOrDefault();");
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> Get(List<{pk.DataTypeString}> {pk.FieldName}s)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"return Get({pk.FieldName}s.ToArray());");
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> Get(List<{pk.DataTypeString}> {pk.FieldName}s, bool skipCache)");
                    sb.AppendLine(Tab2, "{");

                    sb.AppendLine(Tab3, $"var toReturn = new List<{table.ClassName}>();");
                    sb.AppendLine(Tab3, $"if (!{pk.FieldName}s.Any()) return toReturn;");
                    sb.AppendLine(Tab3, "if (CacheEnabled && !skipCache)");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"var cachedIds = new List<{pk.DataTypeString}>();");
                    sb.AppendLine(Tab4, $"foreach (var {pk.FieldName} in {pk.FieldName}s.Where(IsInCache))");
                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, $"cachedIds.Add({pk.FieldName});");
                    sb.AppendLine(Tab5, $"toReturn.Add(GetFromCache({pk.FieldName}));");
                    sb.AppendLine(Tab4, "}");
                    sb.AppendLine(Tab4, $"{pk.FieldName}s = {pk.FieldName}s.Except(cachedIds).ToList();");
                    sb.AppendLine(Tab4, $"if (!{pk.FieldName}s.Any()) return toReturn;");
                    sb.AppendLine(Tab3, "}");


                    sb.AppendLine(Tab3, $"toReturn.AddRange(Where({(pk.DbColumnName == nameof(pk.DbColumnName) ? $"nameof({ table.ClassName}.{ pk.DbColumnName})" : $"\"{pk.DbColumnName}\"")}, Comparison.In, { pk.FieldName}s).Results()); ");

                    sb.AppendLine(Tab3, $"return toReturn;");

                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> Get(params {pk.DataTypeString}[] {pk.FieldName}s)");
                    sb.AppendLine(Tab2, "{");

                    sb.AppendLine(Tab3, $"return Get({pk.FieldName}s.ToList(), false);");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                }


                if (table.PrimaryKeys.Count == 1 &&
                    new[] { typeof(short), typeof(int), typeof(long), typeof(decimal), typeof(double), typeof(float) }
                        .Contains(pk.DataType))
                {
                    //Get Max ID
                    sb.AppendLine(Tab2, $"public {pk.DataTypeString} GetMaxId()");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, "using (var cn = new SqlConnection(ConnectionString))");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4,
                        $"using (var cmd = CreateCommand(cn, \"SELECT ISNULL(MAX({pk.DbColumnName}), 0) FROM {table.DbTableName}\"))");
                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, "cn.Open();");
                    sb.AppendLine(Tab5, $"return ({pk.DataTypeString})cmd.ExecuteScalar();");
                    sb.AppendLine(Tab4, "}");
                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab2, "}");
                }
            }

            if (inherits)
            {
                var pk = table.PrimaryKeys.First();

                sb.AppendLine(Tab2, $"public override IEnumerable<{table.ClassName}> GetAll()");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, "var query = $@\"SELECT * FROM ");
                sb.AppendLine(Tab8, $"[{table.DbTableName}] {table.DbTableName.ToLower()[0]}");
                var inheritedTable = table;
                var originalAlias = table.DbTableName.ToLower()[0];
                var previousAlias = table.DbTableName.ToLower()[0];
                var inheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
                do
                {
                    sb.AppendLine(Tab7, $"LEFT JOIN [{inheritedDependency.ForeignKeyTargetTable}] {inheritedDependency.ForeignKeyTargetTable.ToLower()[0]}");
                    sb.AppendLine(Tab8,
                        $"ON {previousAlias}.{pk.DbColumnName} = {inheritedDependency.ForeignKeyTargetTable.ToLower()[0]}.{pk.DbColumnName}");

                    previousAlias = inheritedDependency.ForeignKeyTargetTable.ToLower()[0];
                    inheritedTable = otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
                    inheritedDependency = inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
                } while (inheritedDependency != null);

                sb.AppendLine(Tab7, "\";");
                sb.AppendLine(Tab3, "return ExecuteSql(query); ");
                sb.AppendLine(Tab2, "}");
            }

            return sb;
        }

        private StringBuilder Repo_Create(Table table, List<Table> otherTables, bool inherits)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));

            var sb = new StringBuilder();

            sb.AppendLine(Tab2, $"public override bool Create({table.ClassName} item)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "//Validation");
            sb.AppendLine(Tab3, "if (item == null)");
            sb.AppendLine(Tab4, "return false;");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "var validationErrors = item.Validate();");
            sb.AppendLine(Tab3, "if (validationErrors.Any())");
            sb.AppendLine(Tab4, "throw new ValidationException(validationErrors);");
            sb.AppendLine("");

            if (inherits)
            {
                sb.AppendLine(Tab3, $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Create(item))");
                sb.AppendLine(Tab3, "{");
            }

            sb.Append(inherits ? Tab4 : Tab3, "var createdKeys = BaseCreate(");
            sb.Append(PrintBlockScopedVariables(table.Columns));
            sb.AppendLine(");");

            sb.AppendLine(inherits ? Tab4 : Tab3, $"if (createdKeys.Count != {table.ClassName}.Columns.Count(x => x.PrimaryKey))");
            sb.AppendLine(inherits ? Tab5 : Tab4, "return false;");
            sb.AppendLine("");
            foreach (var pk in table.PrimaryKeys)
            {
                sb.AppendLine(inherits ? Tab4 : Tab3,
                    $"item.{pk.PropertyName} = ({pk.DataTypeString})createdKeys[nameof({table.ClassName}.{pk.PropertyName})];");
            }

            sb.AppendLine(inherits ? Tab4 : Tab3, "item.ResetDirty();");
            sb.AppendLine("");
            sb.AppendLine(Tab4, "if (CacheEnabled)");
            sb.AppendLine(Tab4, "{");
            sb.AppendLine(Tab5, "SaveToCache(item);");
            sb.AppendLine(Tab4, "}");
            sb.AppendLine(inherits ? Tab4 : Tab3, "return true;");

            if (inherits)
            {
                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab3, "return false;");
            }
            sb.AppendLine(Tab2, "}");


            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public override bool BulkCreate(params {table.ClassName}[] items)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "if (!items.Any())");
            sb.AppendLine(Tab4, "return false;");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "var validationErrors = items.SelectMany(x => x.Validate()).ToList();");
            sb.AppendLine(Tab3, "if (validationErrors.Any())");
            sb.AppendLine(Tab4, "throw new ValidationException(validationErrors);");
            sb.AppendLine("");
            if (inherits)
            {
                sb.AppendLine(Tab3, $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.BulkCreate(items))");
                sb.AppendLine(Tab3, "{");
            }
            sb.AppendLine(inherits ? Tab4 : Tab3, "var dt = new DataTable();");
            sb.AppendLine(inherits ? Tab4 : Tab3,
                $"foreach (var mergeColumn in {table.ClassName}.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))");
            sb.AppendLine(inherits ? Tab5 : Tab4, "dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);");
            sb.AppendLine("");
            sb.AppendLine(inherits ? Tab4 : Tab3, "foreach (var item in items)");
            sb.AppendLine(inherits ? Tab4 : Tab3, "{");


            sb.Append(inherits ? Tab5 : Tab4, "dt.Rows.Add(");
            sb.Append(PrintBlockScopedVariables(table.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.IsIdentity)
                .ToList()));
            sb.AppendLine("); ");
            sb.AppendLine(inherits ? Tab4 : Tab3, "}");
            sb.AppendLine("");

            sb.AppendLine(inherits ? Tab4 : Tab3, "if (BulkInsert(dt))");
            sb.AppendLine(inherits ? Tab4 : Tab3, "{");
            sb.AppendLine(inherits ? Tab5 : Tab4, "if (CacheEnabled)");
            sb.AppendLine(inherits ? Tab5 : Tab4, "{");
            sb.AppendLine(inherits ? Tab6 : Tab5, "foreach (var item in items)");
            sb.AppendLine(inherits ? Tab6 : Tab5, "{");
            sb.AppendLine(inherits ? Tab7 : Tab6, "SaveToCache(item);");
            sb.AppendLine(inherits ? Tab6 : Tab5, "}");
            sb.AppendLine(inherits ? Tab5 : Tab4, "}");
            sb.AppendLine(inherits ? Tab5 : Tab4, "return true;");
            sb.AppendLine(inherits ? Tab4 : Tab3, "}");
            sb.AppendLine(inherits ? Tab4 : Tab3, "return false;");
            if (inherits)
            {
                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab3, "return false;");
            }
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"public override bool BulkCreate(List<{table.ClassName}> items)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "return BulkCreate(items.ToArray());");
            sb.AppendLine(Tab2, "}");


            return sb;
        }

        private StringBuilder Repo_Update(Table table, List<Table> otherTables, bool inherits)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));

            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public bool Update({table.ClassName} item, bool clearDirty = true)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "if (item == null)");
            sb.AppendLine(Tab4, "return false;");

            sb.AppendLine(Tab3, "if (CacheEnabled)");
            sb.AppendLine(Tab3, "{");
            var pk = table.PrimaryKeys.First();
            sb.AppendLine(Tab4, $"RemoveFromCache(item.{pk.DbColumnName});");
            sb.AppendLine(Tab3, "}");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "var validationErrors = item.Validate();");
            sb.AppendLine(Tab3, "if (validationErrors.Any())");
            sb.AppendLine(Tab4, "throw new ValidationException(validationErrors);");
            sb.AppendLine("");

            if (inherits)
            {
                sb.AppendLine(Tab3,
                    $"var success = _{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Update(item, false);");
                sb.AppendLine(Tab3, "success &= BaseUpdate(item.DirtyColumns, ");
            }
            else
            {
                sb.AppendLine(Tab3, "var success = BaseUpdate(item.DirtyColumns, ");
            }

            sb.Append(Tab4);
            sb.Append(PrintBlockScopedVariables(table.Columns));

            sb.AppendLine(");");
            sb.AppendLine("");

            sb.AppendLine(Tab3, "if (success && clearDirty)");
            sb.AppendLine(Tab4, "item.ResetDirty();");

            sb.AppendLine(Tab3, "if (success && CacheEnabled)");
            sb.AppendLine(Tab3, "{");
            sb.AppendLine(Tab4, "SaveToCache(item);");
            sb.AppendLine(Tab3, "}");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "return success;");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private StringBuilder Repo_Delete(Table table, List<Table> otherTables, bool inherits)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));

            var sb = new StringBuilder();

            sb.AppendLine(Tab2, $"public bool Delete({table.ClassName} {table.LowerClassName})");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"if ({table.LowerClassName} == null)");
            sb.AppendLine(Tab4, "return false;");
            sb.AppendLine("");
            var tpk = table.PrimaryKeys[0];
            sb.AppendLine(Tab3,
                $"var deleteColumn = new DeleteColumn(\"{tpk.DbColumnName}\", {table.LowerClassName}.{tpk.DbColumnName}, SqlDbType.{tpk.DbType});");
            sb.AppendLine("");
            if (inherits)
            {
                sb.AppendLine(Tab3, "if (BaseDelete(deleteColumn, out var items))");
                sb.AppendLine(Tab3, "{");
                sb.AppendLine(Tab4, $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Delete({table.LowerClassName}))");
                sb.AppendLine(Tab4, "{");

                sb.AppendLine(Tab5, "if (CacheEnabled)");
                sb.AppendLine(Tab5, "{");
                sb.AppendLine(Tab6, $"RemoveFromCache({table.LowerClassName}.{tpk.DbColumnName});");
                sb.AppendLine(Tab5, "}");
                sb.AppendLine(Tab5, "return true;");
                sb.AppendLine(Tab4, "}");

                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab3, "return false;");
            }
            else
            {
                sb.AppendLine(Tab3, "return BaseDelete(deleteColumn, out var items);");
            }
            sb.AppendLine(Tab2, "}");

            if (table.PrimaryKeyConfiguration != PrimaryKeyConfigurationEnum.CompositeKey)
            {
                sb.AppendLine(Tab2, $"public bool Delete(IEnumerable<{table.ClassName}> items)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, "if (!items.Any()) return true;");
                sb.AppendLine(Tab3, "var deleteValues = new List<object>();");
                sb.AppendLine(Tab3, "foreach (var item in items)");
                sb.AppendLine(Tab3, "{");
                sb.AppendLine(Tab4, $"deleteValues.Add(item.{table.PrimaryKeys[0].DbColumnName});");
                sb.AppendLine(Tab3, "}");
                sb.AppendLine("");

                if (inherits)
                {
                    sb.AppendLine(Tab3, $"if (BaseDelete(\"{table.PrimaryKeys[0].DbColumnName}\", deleteValues))");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Delete(items))");


                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, "if (CacheEnabled)");
                    sb.AppendLine(Tab5, "{");

                    sb.AppendLine(Tab6, "foreach (var item in items)");
                    sb.AppendLine(Tab6, "{");
                    sb.AppendLine(Tab7, $"RemoveFromCache(item.{tpk.DbColumnName});");
                    sb.AppendLine(Tab6, "}");

                    sb.AppendLine(Tab5, "}");
                    sb.AppendLine(Tab5, "return true;");
                    sb.AppendLine(Tab4, "}");


                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab3, "return false;");
                }
                else
                {
                    sb.AppendLine(Tab3, $"return BaseDelete(\"{table.PrimaryKeys[0].DbColumnName}\", deleteValues);");
                }

                sb.AppendLine(Tab2, "}");
            }

            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                var pkParamList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.DataTypeString} {column.FieldName}, ")
                    .TrimEnd(' ', ',');

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public bool Delete({pkParamList})");
                sb.AppendLine(Tab2, "{");
                sb.Append(Tab3, $"return Delete(new {table.ClassName} {{ ");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append($"{pk.PropertyName} = {pk.FieldName}");
                    if (pk != table.PrimaryKeys.Last())
                        sb.Append(",");
                }

                sb.AppendLine("});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2, $"public bool Delete({table.ClassName}Keys compositeId)");
                sb.AppendLine(Tab2, "{");
                sb.Append(Tab3, $"return Delete(new {table.ClassName} {{ ");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append($"{pk.PropertyName} = compositeId.{pk.PropertyName}");
                    if (pk != table.PrimaryKeys.Last())
                        sb.Append(",");
                }

                sb.AppendLine("});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");

                sb.AppendLine(Tab2,
                    $"public bool Delete(IEnumerable<{table.ClassName}Keys> compositeIds)");
                sb.AppendLine(Tab2, "{");

                sb.AppendLine(Tab3, "var tempTableName = $\"staging{DateTime.Now.Ticks}\";");
                sb.AppendLine(Tab3, "var dt = new DataTable();");
                sb.AppendLine(Tab3, $"foreach (var mergeColumn in {table.ClassName}.Columns.Where(x => x.PrimaryKey))");
                sb.AppendLine(Tab3, "{");
                sb.AppendLine(Tab4, "dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType);");
                sb.AppendLine(Tab3, "}");

                sb.AppendLine(Tab3, "foreach (var compositeId in compositeIds)");
                sb.AppendLine(Tab3, "{");
                sb.Append(Tab4, "dt.Rows.Add(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append($"compositeId.{pk.PropertyName}");
                    if (pk != table.PrimaryKeys.Last())
                        sb.Append(",");
                }

                sb.AppendLine(");");
                sb.AppendLine(Tab3, "}");

                sb.AppendLine(Tab3, "CreateStagingTable(tempTableName, true);");
                sb.AppendLine(Tab3, "BulkInsert(dt, tempTableName);");

                sb.AppendLine(Tab3, "using (var cn = new SqlConnection(ConnectionString))");
                sb.AppendLine(Tab3, "{");
                sb.Append(Tab4, "using (var cmd = CreateCommand(cn, $@\";WITH cte AS (");
                sb.AppendLine($"SELECT * FROM {table.Schema}.{table.DbTableName} o");
                sb.Append(Tab7, "WHERE EXISTS (SELECT 'x' FROM {tempTableName} i WHERE ");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append($"i.[{pk.PropertyName}] = o.[{pk.PropertyName}]");
                    if (pk != table.PrimaryKeys.Last())
                        sb.Append(" AND ");
                }

                sb.AppendLine("))");

                sb.AppendLine(Tab7, "DELETE FROM cte\"))");

                sb.AppendLine(Tab4, "{");
                sb.AppendLine(Tab5, "try");
                sb.AppendLine(Tab5, "{");
                sb.AppendLine(Tab6, "cn.Open();");
                sb.AppendLine(Tab6, "return (int)cmd.ExecuteScalar() > 0;");
                sb.AppendLine(Tab5, "}");
                sb.AppendLine(Tab5, "finally { cn.Close(); }");
                sb.AppendLine(Tab4, "}");

                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
            }
            else if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.PrimaryKey)
            {
                var pk = table.PrimaryKeys.First();

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public bool Delete({pk.DataTypeString} {pk.FieldName})");
                sb.AppendLine(Tab2, "{");
                if (inherits)
                {

                    sb.AppendLine(Tab3, $"if (Delete(new {table.ClassName} {{ {pk.PropertyName} = {pk.FieldName} }}))");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Delete({pk.FieldName}))");
                    sb.AppendLine(Tab4, "{");


                    sb.AppendLine(Tab5, "if (CacheEnabled)");
                    sb.AppendLine(Tab5, "{");
                    sb.AppendLine(Tab6, $"RemoveFromCache({pk.FieldName});");
                    sb.AppendLine(Tab5, "}");
                    sb.AppendLine(Tab5, "return true;");
                    sb.AppendLine(Tab4, "}");



                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab3, "return false;");
                }
                else
                {
                    sb.AppendLine(Tab3, $"return Delete(new {table.ClassName} {{ {pk.PropertyName} = {pk.FieldName} }});");
                }
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");


                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public bool Delete(IEnumerable<{pk.DataTypeString}> {pk.FieldName}s)");
                sb.AppendLine(Tab2, "{");

                sb.AppendLine(Tab3, $"if (!{pk.FieldName}s.Any()) return true;");
                sb.AppendLine(Tab3, "var deleteValues = new List<object>();");
                sb.AppendLine(Tab3, $"deleteValues.AddRange({pk.FieldName}s.Cast<object>()); ");
                if (inherits)
                {

                    sb.AppendLine(Tab3, $"if (BaseDelete(\"{table.PrimaryKeys[0].DbColumnName}\", deleteValues))");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Delete({pk.FieldName}s))");

                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, $"if (CacheEnabled)");
                    sb.AppendLine(Tab5, "{");
                    sb.AppendLine(Tab6, $"foreach (var {pk.FieldName} in {pk.FieldName}s)");
                    sb.AppendLine(Tab6, "{");
                    sb.AppendLine(Tab7, $"RemoveFromCache({pk.FieldName});");
                    sb.AppendLine(Tab6, "}");
                    sb.AppendLine(Tab5, "}");
                    sb.AppendLine(Tab4, $"");
                    sb.AppendLine(Tab5, $"return true;");
                    sb.AppendLine(Tab4, "}");

                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab3, "return false;");
                }
                else
                {
                    sb.AppendLine(Tab3, $"return BaseDelete(\"{table.PrimaryKeys[0].DbColumnName}\", deleteValues);");
                }
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
            }

            return sb;
        }

        private string Repo_DeleteBy(Table table, List<Table> otherTables, bool inherits)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
            var sb = new StringBuilder();

            var pk = table.PrimaryKeys.First();
            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey || inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName) continue;

                sb.AppendLine(Tab2,
                    $"public bool DeleteBy{column.DbColumnName}({column.DataTypeString} {column.FieldName})");
                sb.AppendLine(Tab2, "{");

                sb.AppendLine(Tab3, $"if (BaseDelete(new DeleteColumn(\"{column.DbColumnName}\", {column.FieldName}, SqlDbType.{column.DbType}), out var items))");
                sb.AppendLine(Tab3, "{");
                sb.AppendLine(Tab4, "if (CacheEnabled)");
                sb.AppendLine(Tab4, "{");
                sb.AppendLine(Tab5, "foreach (var item in items)");
                sb.AppendLine(Tab5, "{");
                sb.AppendLine(Tab6, $"RemoveFromCache(item.{pk.DbColumnName});");
                sb.AppendLine(Tab5, "}");
                sb.AppendLine(Tab4, "}");
                sb.AppendLine(Tab4, $"return true;");
                sb.AppendLine(Tab3, "}");

                sb.AppendLine(Tab3, $"return false;");

                sb.AppendLine(Tab2, "}");
            }

            if (inherits)
            {
                //DeleteBy for base Tables
                AppendDeleteByImplementationForInherited(inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst(), otherTables, inheritedDependency, sb);
            }

            return sb.ToString();
        }

        private void AppendDeleteByImplementationForInherited(string repository, List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                var pk = inheritedTable.PrimaryKeys.First();
                foreach (var inheritedColumn in inheritedTable.Columns)
                {

                    if (inheritedColumn.PrimaryKey || (inheritedDependency != null && inheritedColumn.DbColumnName == inheritedDependency.DbColumnName)) continue;

                    sb.AppendLine(Tab2,
                        $"public bool DeleteBy{inheritedColumn.DbColumnName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName})");
                    sb.AppendLine(Tab2, "{");

                    sb.AppendLine(Tab3, $"if (BaseDelete(new DeleteColumn(\"{inheritedColumn.DbColumnName}\", {inheritedColumn.FieldName}, SqlDbType.{inheritedColumn.DbType}), out var items))");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, "if (CacheEnabled)");
                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, "foreach (var item in items)");
                    sb.AppendLine(Tab5, "{");
                    sb.AppendLine(Tab6, $"RemoveFromCache(item.{pk.DbColumnName});");
                    sb.AppendLine(Tab5, "}");
                    sb.AppendLine(Tab4, "}");
                    sb.AppendLine(Tab4, $"return true;");
                    sb.AppendLine(Tab3, "}");

                    sb.AppendLine(Tab3, $"return false;");

                    sb.AppendLine(Tab2, "}");
                }

                if (inheritedTableAlsoInherits)
                {
                    AppendDeleteByImplementationForInherited(repository, otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private StringBuilder Repo_Merge(Table table, List<Table> otherTables, Column inheritedDependency)
        {
            var inherits = inheritedDependency != null;
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public bool Merge(List<{table.ClassName}> items)");
            sb.AppendLine(Tab2, "{");

            if (inherits)
            {
                var inheritedTable =
                    otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
                sb.AppendLine(Tab3,
                    $"if (_{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.Merge(items.Cast<{inheritedTable.ClassName}>().ToList()))");

                sb.AppendLine(Tab3, "{");
            }
            sb.AppendLine(inherits ? Tab4 : Tab3, "var mergeTable = new List<object[]>();");

            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.NoKey)
            {
                sb.AppendLine(inherits ? Tab4 : Tab3, "foreach (var item in items)");
            }
            else if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                sb.AppendLine(inherits ? Tab4 : Tab3, $"foreach (var item in items.GroupBy(x => new {{{ string.Join(",", table.PrimaryKeys.Select(x => $"x.{x.PropertyName}")) }}}).Select(x => x.Last()))");
            }
            else if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.PrimaryKey)
            {
                sb.AppendLine(inherits ? Tab4 : Tab3,
                    $"var updates = items.Where(x => x.{table.PrimaryKeys[0].PropertyName} != default).GroupBy(x => x.{table.PrimaryKeys[0].PropertyName}).Select(x => x.Last());");
                sb.AppendLine(inherits ? Tab4 : Tab3, $"foreach (var item in items.Where(x => x.{table.PrimaryKeys[0].PropertyName} == default).Union(updates))");
            }

            sb.AppendLine(inherits ? Tab4 : Tab3, "{");
            sb.AppendLine(inherits ? Tab5 : Tab4, "mergeTable.Add(new object[]");
            sb.AppendLine(inherits ? Tab5 : Tab4, "{");

            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey)
                    sb.Append(inherits ? Tab6 : Tab5, $"item.{column.PropertyName}");
                else
                {
                    sb.Append(inherits ? Tab6 : Tab5,
                        $"item.{column.PropertyName}, item.DirtyColumns.Contains({(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")})");
                }

                sb.AppendLine(column != table.Columns.Last() ? "," : "");
            }

            sb.AppendLine(inherits ? Tab5 : Tab4, "});");
            sb.AppendLine(inherits ? Tab4 : Tab3, "}");
            sb.AppendLine(inherits ? Tab4 : Tab3, "return BaseMerge(mergeTable);");

            if (inherits)
            {
                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab3, "return false;");
            }


















            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, "public bool Merge(string csvPath)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var mergeTable = new List<object[]>();");
            sb.AppendLine(Tab3, "using (var sr = new StreamReader(csvPath))");
            sb.AppendLine(Tab3, "{");
            sb.AppendLine(Tab4, "var line = sr.ReadLine();");
            sb.AppendLine(Tab4, "if (line == null) return false;");
            sb.AppendLine();
            sb.AppendLine(Tab4, "var firstItem = line.Split(',')[0];");
            sb.AppendLine(Tab4, $"if (firstItem == \"{table.Columns.First().PropertyName}\")");
            sb.AppendLine(Tab4, "{");
            sb.AppendLine(Tab5, "//CSV has headers");
            sb.AppendLine(Tab5, "//Run to the next line");
            sb.AppendLine(Tab5, "line = sr.ReadLine();");
            sb.AppendLine(Tab5, "if (line == null) return true;");
            sb.AppendLine(Tab4, "}");
            sb.AppendLine();
            sb.AppendLine(Tab4, "do");
            sb.AppendLine(Tab4, "{");
            sb.AppendLine(Tab5, "var blocks = line.Split(',');");


            sb.AppendLine(Tab5, "mergeTable.Add(new object[]");
            sb.AppendLine(Tab5, "{");
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];

                if (column.PrimaryKey)
                {
                    sb.AppendLine(Tab6, $"Cast<{column.DataTypeString}>(blocks[{i}]),");
                }
                else
                {
                    sb.AppendLine(Tab6, $"Cast<{column.DataTypeString}>(blocks[{i}]), true,");
                }
            }

            sb.AppendLine(Tab5, "});");
            sb.AppendLine(Tab4, "} while ((line = sr.ReadLine()) != null);");
            sb.AppendLine();
            sb.AppendLine(Tab4, "");
            sb.AppendLine(Tab4, "return BaseMerge(mergeTable);");
            sb.AppendLine(Tab3, "}");
            sb.AppendLine(Tab2, "}");


            return sb;
        }

        private StringBuilder Repo_ToItem(Table table, Column inheritedDependency)
        {
            var inherits = inheritedDependency != null;
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public override {table.ClassName} ToItem(DataRow row, bool skipBase)");
            sb.AppendLine(Tab2, "{");

            if (inherits)
            {
                sb.AppendLine(Tab3, $"var item = skipBase ? new {table.ClassName}() : _{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.ToItem<{table.ClassName}>(row, false);");

                foreach (var column in table.Columns)
                {
                    sb.AppendLine(Tab3,
                        $"item.{column.PropertyName} = Get{(IsCSharpNullable(column.DataType.Name) && column.IsNullable ? "Nullable" : "")}{(column.DataType.Name.Contains("[]") ? column.DataType.Name.Replace("[]", "Array") : column.DataType.Name)}(row, {(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")});");
                }
            }
            else
            {
                sb.AppendLine(Tab3, $"var item = new {table.ClassName}");
                sb.AppendLine(Tab3, "{");

                foreach (var column in table.Columns)
                {
                    sb.AppendLine(Tab4,
                        $"{column.PropertyName} = Get{(IsCSharpNullable(column.DataType.Name) && column.IsNullable ? "Nullable" : "")}{(column.DataType.Name.Contains("[]") ? column.DataType.Name.Replace("[]", "Array") : column.DataType.Name)}(row, {(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")}),");
                }

                sb.AppendLine(Tab3, "};");
            }

            sb.AppendLine("");
            sb.AppendLine(Tab3, "item.ResetDirty();");
            sb.AppendLine(Tab3, "return item;");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine("");
            sb.AppendLine(Tab2, "public override TK ToItem<TK>(DataRow row, bool skipBase)");
            sb.AppendLine(Tab2, "{");


            if (inherits)
            {
                sb.AppendLine(Tab3, $"var item = skipBase ? new {table.ClassName}() : _{inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst()}.ToItem<TK>(row, false);");

                foreach (var column in table.Columns)
                {
                    sb.AppendLine(Tab3,
                        $"item.{column.PropertyName} = Get{(IsCSharpNullable(column.DataType.Name) && column.IsNullable ? "Nullable" : "")}{(column.DataType.Name.Contains("[]") ? column.DataType.Name.Replace("[]", "Array") : column.DataType.Name)}(row, {(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")});");
                }
            }
            else
            {
                sb.AppendLine(Tab3, "var item = new TK");
                sb.AppendLine(Tab3, "{");

                foreach (var column in table.Columns)
                {
                    sb.AppendLine(Tab4,
                        $"{column.PropertyName} = Get{(IsCSharpNullable(column.DataType.Name) && column.IsNullable ? "Nullable" : "")}{(column.DataType.Name.Contains("[]") ? column.DataType.Name.Replace("[]", "Array") : column.DataType.Name)}(row, {(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")}),");
                }

                sb.AppendLine(Tab3, "};");
            }

            sb.AppendLine("");
            sb.AppendLine(Tab3, "item.ResetDirty();");
            sb.AppendLine(Tab3, "return item as TK;");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private StringBuilder Repo_Search(Table table, List<Table> otherTables, Column inheritedDependency)
        {
            var inherits = inheritedDependency != null;
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> Search(");

            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey || (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName)) continue;

                sb.Append(Tab3,
                    column.DataType != typeof(XmlDocument)
                        ? $"{column.DataTypeString}{(IsCSharpNullable(column.DataTypeString) ? "?" : string.Empty)} {column.FieldName} = null"
                        : $"String {column.FieldName} = null");

                if (column != table.Columns.Last())
                    sb.AppendLine(",");
            }
            if (inherits)
            {
                AppendSearchByForInherited(otherTables, inheritedDependency, sb);
            }

            sb.AppendLine(")");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var queries = new List<QueryItem>(); ");
            sb.AppendLine("");
            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey || (inheritedDependency != null && column.DbColumnName == inheritedDependency.DbColumnName)) continue;

                if (IsCSharpNullable(column.DataTypeString))
                {
                    sb.AppendLine(Tab3, $"if ({column.FieldName}.HasValue)");
                }
                else
                    switch (column.DataTypeString)
                    {
                        case "String":
                            sb.AppendLine(Tab3, $"if (!string.IsNullOrEmpty({column.FieldName}))");
                            break;
                        case "Byte[]":
                            sb.AppendLine(Tab3, $"if ({column.FieldName}.Any())");
                            break;
                        default:
                            sb.AppendLine(Tab3, $"if ({column.FieldName} != null)");
                            break;
                    }

                if (column.DataType != typeof(XmlDocument))
                {
                    sb.AppendLine(Tab4,
                        $"queries.Add(new QueryItem({(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")}, {column.FieldName}));");
                }
                else
                {
                    sb.AppendLine(Tab4,
                        $"queries.Add(new QueryItem({(column.DbColumnName == nameof(column.DbColumnName) ? $"nameof({table.ClassName}.{column.DbColumnName})" : $"\"{column.DbColumnName}\"")}, {column.FieldName}, typeof(XmlDocument)));");
                }
            }

            if (inherits)
            {
                AppendSearchByImplementationForInherited(table, otherTables, inheritedDependency, sb);
            }

            sb.AppendLine("");
            sb.AppendLine(Tab3, "return BaseSearch(queries);");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private void AppendSearchByImplementationForInherited(Table table, List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedColumn.PrimaryKey || (inheritedDependency != null && inheritedColumn.DbColumnName == inheritedDependency.DbColumnName)) continue;

                    if (IsCSharpNullable(inheritedColumn.DataTypeString))
                    {
                        sb.AppendLine(Tab3, $"if ({inheritedColumn.FieldName}.HasValue)");
                    }
                    else
                        switch (inheritedColumn.DataTypeString)
                        {
                            case "String":
                                sb.AppendLine(Tab3, $"if (!string.IsNullOrEmpty({inheritedColumn.FieldName}))");
                                break;
                            case "Byte[]":
                                sb.AppendLine(Tab3, $"if ({inheritedColumn.FieldName}.Any())");
                                break;
                            default:
                                sb.AppendLine(Tab3, $"if ({inheritedColumn.FieldName} != null)");
                                break;
                        }

                    if (inheritedColumn.DataType != typeof(XmlDocument))
                    {
                        sb.AppendLine(Tab4,
                            $"queries.Add(new QueryItem({(inheritedColumn.DbColumnName == nameof(inheritedColumn.DbColumnName) ? $"nameof({table.ClassName}.{inheritedColumn.DbColumnName})" : $"\"{inheritedColumn.DbColumnName}\"")}, {inheritedColumn.FieldName}));");
                    }
                    else
                    {
                        sb.AppendLine(Tab4,
                            $"queries.Add(new QueryItem({(inheritedColumn.DbColumnName == nameof(inheritedColumn.DbColumnName) ? $"nameof({table.ClassName}.{inheritedColumn.DbColumnName})" : $"\"{inheritedColumn.DbColumnName}\"")}, {inheritedColumn.FieldName}, typeof(XmlDocument)));");
                    }
                }

                if (inheritedTableAlsoInherits)
                {
                    AppendSearchByImplementationForInherited(table, otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        private StringBuilder Repo_Find(Table table, List<Table> otherTables, bool inherited)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));
            var sb = new StringBuilder();

            if (table.PrimaryKeyConfiguration == PrimaryKeyConfigurationEnum.CompositeKey)
            {
                sb.AppendLine("");
                //Find methods on PK'S are available as there's a composite primary key
                foreach (var primaryKey in table.PrimaryKeys)
                {
                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> FindBy{primaryKey.DbColumnName}({primaryKey.DataTypeString} {primaryKey.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3,
                        $"var items = FindBy{primaryKey.PropertyName}(FindComparison.Equals, {primaryKey.FieldName});");

                    sb.AppendLine(Tab3, "if (CacheEnabled && !skipCache)");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"foreach (var item in items)");
                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, $"SaveToCache(item);");
                    sb.AppendLine(Tab4, "}");
                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab3, $"return items;");
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        $"public IEnumerable<{table.ClassName}> FindBy{primaryKey.DbColumnName}(FindComparison comparison, {primaryKey.DataTypeString} {primaryKey.FieldName})");
                    sb.AppendLine(Tab2, "{");

                    sb.AppendLine(Tab3,
                        $"return Where({(primaryKey.PropertyName == nameof(primaryKey.PropertyName) ? $"nameof({table.ClassName}.{primaryKey.PropertyName})" : $"\"{primaryKey.PropertyName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {primaryKey.FieldName}).Results();");
                    sb.AppendLine(Tab2, "}");
                }
            }

            foreach (var nonPrimaryKey in table.NonPrimaryKeys)
            {
                if (nonPrimaryKey.PrimaryKey || (inheritedDependency != null && nonPrimaryKey.DbColumnName == inheritedDependency.DbColumnName)) continue;
                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName})"
                        : $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3,
                    $"return FindBy{nonPrimaryKey.PropertyName}(FindComparison.Equals, {nonPrimaryKey.FieldName}, false);");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName}, bool skipCache)"
                        : $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName}, bool skipCache)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3,
                    $"return FindBy{nonPrimaryKey.PropertyName}(FindComparison.Equals, {nonPrimaryKey.FieldName}, skipCache);");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");


                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName})"
                        : $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3,
                    $"return FindBy{nonPrimaryKey.PropertyName}(comparison, {nonPrimaryKey.FieldName}, false);");

                sb.AppendLine(Tab2, "}");





                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataTypeString} {nonPrimaryKey.FieldName}, bool skipCache)"
                        : $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName}, bool skipCache)");
                sb.AppendLine(Tab2, "{");
                if (nonPrimaryKey.DataType != typeof(XmlDocument))
                {
                    sb.AppendLine(Tab3,
                        $"var items = Where({(nonPrimaryKey.DbColumnName == nameof(nonPrimaryKey.DbColumnName) ? $"nameof({table.ClassName}.{nonPrimaryKey.DbColumnName})" : $"\"{nonPrimaryKey.DbColumnName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {nonPrimaryKey.FieldName}).Results();");

                    sb.AppendLine(Tab3, "if (CacheEnabled && !skipCache)");
                    sb.AppendLine(Tab3, "{");
                    sb.AppendLine(Tab4, $"foreach (var item in items)");
                    sb.AppendLine(Tab4, "{");
                    sb.AppendLine(Tab5, $"SaveToCache(item);");
                    sb.AppendLine(Tab4, "}");
                    sb.AppendLine(Tab3, "}");
                    sb.AppendLine(Tab3, $"return items;");
                }
                else
                {
                    sb.AppendLine(Tab3,
                        $"return Where({(nonPrimaryKey.DbColumnName == nameof(nonPrimaryKey.DbColumnName) ? $"nameof({table.ClassName}.{nonPrimaryKey.DbColumnName})" : $"\"{nonPrimaryKey.DbColumnName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {nonPrimaryKey.FieldName}, typeof(XmlDocument)).Results();");
                }

                sb.AppendLine(Tab2, "}");
            }

            if (inherited)
            {
                AppendFindByImplementationForInherited(inheritedDependency.ForeignKeyTargetTable.ToRepositoryName(_generationSettings.RepositoryClassNameFormat).LowerFirst(), table.ClassName, otherTables, inheritedDependency, sb);
            }

            return sb;
        }

        private void AppendFindByImplementationForInherited(string repository, string className, List<Table> otherTables, Column inheritedDependency, StringBuilder sb)
        {
            var inheritedTable =
                otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var pk = inheritedTable.PrimaryKeys.First();
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                foreach (var inheritedColumn in inheritedTable.Columns)
                {
                    if (inheritedColumn.PrimaryKey || (inheritedDependency != null && inheritedColumn.DbColumnName == inheritedDependency.DbColumnName)) continue;
                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName})"
                            : $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(String {inheritedColumn.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3,
                        $"return FindBy{inheritedColumn.PropertyName}(FindComparison.Equals, {inheritedColumn.FieldName});");
                    sb.AppendLine(Tab2, "}");
                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}({inheritedColumn.DataTypeString} {inheritedColumn.FieldName}, bool skipCache)"
                            : $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(String {inheritedColumn.FieldName}, bool skipCache)");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3,
                        $"return FindBy{inheritedColumn.PropertyName}(FindComparison.Equals, {inheritedColumn.FieldName}, skipCache);");
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, {inheritedColumn.DataTypeString} {inheritedColumn.FieldName})"
                            : $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, String {inheritedColumn.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3,
                        $"return FindBy{inheritedColumn.PropertyName}(comparison, {inheritedColumn.FieldName}, false);");

                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2,
                        inheritedColumn.DataType != typeof(XmlDocument)
                            ? $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, {inheritedColumn.DataTypeString} {inheritedColumn.FieldName}, bool skipCache)"
                            : $"public IEnumerable<{className}> FindBy{inheritedColumn.PropertyName}(FindComparison comparison, String {inheritedColumn.FieldName}, bool skipCache)");
                    sb.AppendLine(Tab2, "{");
                    if (inheritedColumn.DataType != typeof(XmlDocument))
                    {
                        sb.AppendLine(Tab3,
                            $"var items = Where({(inheritedColumn.DbColumnName == nameof(inheritedColumn.DbColumnName) ? $"nameof({className}.{inheritedColumn.DbColumnName})" : $"\"{inheritedColumn.DbColumnName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {inheritedColumn.FieldName}).Results();");

                        sb.AppendLine(Tab3, "if (CacheEnabled && !skipCache)");
                        sb.AppendLine(Tab3, "{");
                        sb.AppendLine(Tab4, $"foreach (var item in items)");
                        sb.AppendLine(Tab4, "{");
                        sb.AppendLine(Tab5, $"SaveToCache(item);");
                        sb.AppendLine(Tab4, "}");
                        sb.AppendLine(Tab3, "}");
                        sb.AppendLine(Tab3, $"return items;");
                    }
                    else
                    {
                        sb.AppendLine(Tab3,
                            $"return Where({(inheritedColumn.DbColumnName == nameof(inheritedColumn.DbColumnName) ? $"nameof({className}.{inheritedColumn.DbColumnName})" : $"\"{inheritedColumn.DbColumnName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {inheritedColumn.FieldName}, typeof(XmlDocument)).Results();");
                    }

                    sb.AppendLine(Tab2, "}");
                }

                if (inheritedTableAlsoInherits)
                {
                    AppendFindByImplementationForInherited(repository, className, otherTables, inheritedTableInheritedDependency, sb);
                }
            }
        }

        #endregion

        #endregion
    }
}