using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Extensions;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using static RepoLite.Common.Helpers;

namespace RepoLite.GeneratorEngine.Generators
{
    public class CSharpSqlServerGenerator : CodeGenerator
    {
        private const int VARIABLE_BLOCK_SCOPE = 5;
        private readonly TargetFramework _targetFramework;
        private readonly CSharpVersion _cSharpVersion;

        //private Func<string, string, string, string> GetColName = (s, table, name) => $"{(s == name ? $"nameof({table}.{name})" : $"\"{name}\"")}";

        [Import("CSharpSqlServerGeneratorImports", typeof(ICSharpSqlServerGeneratorImports))]
        private ICSharpSqlServerGeneratorImports _import;

        public CSharpSqlServerGenerator()
        {
            _targetFramework = AppSettings.Generation.TargetFramework;
            _cSharpVersion = AppSettings.Generation.CSharpVersion;

            //load mef imports
            var fullPluginDirectory = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\{AppSettings.Generation.PluginDirectory}";
            if (!Directory.Exists(fullPluginDirectory))
                return;

            try
            {
                using (var catalog = new AggregateCatalog(new DirectoryCatalog(fullPluginDirectory)))
                {
                    using (var container = new CompositionContainer(catalog))
                    {
                        if (container.Catalog.Parts.Any())
                        {
                            container.ComposeParts(this);
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public override StringBuilder ModelForTable(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Xml;");
            sb.AppendLine($"using {AppSettings.Generation.ModelGenerationNamespace}.Base;");
            if (_targetFramework >= TargetFramework.Framework4)
                sb.AppendLine("using System.ComponentModel.DataAnnotations;");
            if (_targetFramework >= TargetFramework.Framework45)
                sb.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");

            sb.Append(Environment.NewLine);
            sb.AppendLine(CreateModel(table).ToString());
            return sb;
        }

        public override StringBuilder RepositoryForTable(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"using {AppSettings.Generation.RepositoryGenerationNamespace}.Base;");
            sb.AppendLine($"using {AppSettings.Generation.ModelGenerationNamespace};");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Xml;");
            sb.Append(Environment.NewLine);

            sb.AppendLine($"namespace {AppSettings.Generation.RepositoryGenerationNamespace}");
            sb.AppendLine("{");
            if (_import != null)
                sb.Append(_import.GenerateRepoWrapper(table));

            if (table.HasCompositeKey)
            {
                //build up object to use for composite searching

                sb.AppendLine(Tab1, $"public class {table.ClassName}Keys");
                sb.AppendLine(Tab1, "{");


                foreach (var column in table.PrimaryKeys)
                {
                    //Field
                    sb.AppendLine(Tab2, $"public {column.DataType.Name} {column.PropertyName} {{ get; set; }}");
                }

                sb.AppendLine(Tab2, $"public {table.ClassName}Keys() {{}}");

                sb.AppendLine(Tab2, $"public {table.ClassName}Keys(");
                foreach (var column in table.PrimaryKeys)
                {
                    sb.Append(Tab3, $"{column.DataType.Name} {column.FieldName}");
                    sb.AppendLine(column == table.PrimaryKeys.Last() ? ")" : ",");
                }

                sb.AppendLine(Tab2, "{");
                foreach (var column in table.PrimaryKeys)
                {
                    sb.AppendLine(Tab3, $"{column.PropertyName} = {column.FieldName};");
                }

                sb.AppendLine(Tab2, "}");

                sb.AppendLine(Tab1, "}");
            }

            //Interface
            sb.Append(Interface(table));

            //Repo
            sb.AppendLine(Tab1, $"public sealed partial class {table.ClassName}Repository : BaseRepository<{table.ClassName}>, I{table.ClassName}Repository");
            sb.AppendLine(Tab1, "{");

            //ctor
            sb.AppendLine(Tab2, $"public {table.ClassName}Repository(string connectionString) : this(connectionString, exception => {{ }}) {{ }}");
            sb.AppendLine(Tab2, $"public {table.ClassName}Repository(string connectionString, Action<Exception> logMethod) : base(connectionString, logMethod,");
            sb.AppendLine(Tab3, $"\"{table.Schema}\", \"{table.DbTableName}\", {table.Columns.Count})");
            sb.AppendLine(Tab2, "{");
            foreach (var column in table.Columns)
            {
                var sqlPrecisionColumns = new[] { 35, 60, 62, 99, 106, 108, 122, 167, 175, 231, 239 };
                var colLengthVal = sqlPrecisionColumns.Contains(column.SqlDataTypeCode) ? $"({Math.Max(column.MaxLength, column.MaxIntLength)})" : string.Empty;
                sb.AppendLine(Tab3,
                    _cSharpVersion >= CSharpVersion.CSharp6
                        ? $"Columns.Add(new ColumnDefinition({(column.DbColName == nameof(column.DbColName) ? $"nameof({table.ClassName}.{column.DbColName})" : $"\"{column.DbColName}\"")}, \"[{column.SqlDataType}]{colLengthVal}\", {column.IsNullable.ToString().ToLower()}, {column.PrimaryKey.ToString().ToLower()}, {column.IsIdentity.ToString().ToLower()}));"
                        : $"Columns.Add(new ColumnDefinition(\"{column.DbColName}\", \"[{column.SqlDataType}]{colLengthVal}\", {column.IsNullable.ToString().ToLower()}, {column.PrimaryKey.ToString().ToLower()}, {column.IsIdentity.ToString().ToLower()}));");
            }
            sb.AppendLine(Tab2, "}");

            //get
            sb.Append(Repo_Get(table));
            //create
            sb.Append(Repo_Create(table));
            //update
            if (table.PrimaryKeys.Any())
                sb.Append(Repo_Update(table));
            //delete
            if (table.PrimaryKeys.Any())
                sb.Append(Repo_Delete(table));
            //merge
            sb.Append(Repo_Merge(table));
            //toItem
            sb.Append(Repo_ToItem(table));
            //search
            sb.Append(Repo_Search(table));
            //find
            sb.Append(Repo_Find(table));

            sb.AppendLine(Tab1, "}");
            sb.AppendLine("}");
            return sb;
        }

        public override string FileExtension()
        {
            return "cs";
        }

        #region Helpers Methods

        private StringBuilder CreateModel(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"namespace {AppSettings.Generation.ModelGenerationNamespace}");
            sb.AppendLine("{");

            var tableNameAndSchema = table.DbTableName.GetTableAndSchema();

            if (_targetFramework >= TargetFramework.Framework45)
                sb.AppendLine(Tab1, $"[Table(\"{tableNameAndSchema.Table}\", Schema=\"{tableNameAndSchema.Schema}\")]");
            sb.AppendLine(Tab1, $"public partial class {table.ClassName} : BaseModel");
            sb.AppendLine(Tab1, "{");

            foreach (var column in table.Columns)
            {
                //Field
                sb.AppendLine(Tab2, $"private {column.DataType.Name} _{column.FieldName};");
            }

            sb.Append(Environment.NewLine);
            foreach (var column in table.Columns)
            {
                //Property
                if (column.PrimaryKey && _targetFramework >= TargetFramework.Framework4)
                    sb.AppendLine(Tab2, "[Key]");
                var fieldName = $"_{column.FieldName}";
                sb.AppendLine(Tab2, $"public virtual {column.DataType.Name} {column.PropertyName}");
                sb.AppendLine(Tab2, "{");
                if (_cSharpVersion >= CSharpVersion.CSharp7)
                {
                    sb.AppendLine(Tab3, $"get => {fieldName};");
                    sb.AppendLine(Tab3,
                        _targetFramework >= TargetFramework.Framework45
                            ? $"set => SetValue(ref {fieldName}, value);"
                            : $"set => SetValue(ref {fieldName}, value, nameof({column.PropertyName}));");
                }
                else if (_cSharpVersion == CSharpVersion.CSharp6)
                {
                    sb.AppendLine(Tab3, $"get {{ return {fieldName}; }}");
                    sb.AppendLine(Tab3,
                        _targetFramework >= TargetFramework.Framework45
                            ? $"set {{ SetValue(ref {fieldName}, value); }}"
                            : $"set {{ SetValue(ref {fieldName}, value, nameof({column.PropertyName})); }}");
                }
                else
                {
                    sb.AppendLine(Tab3, $"get {{ return {fieldName}; }}");
                    sb.AppendLine(Tab3,
                        _targetFramework >= TargetFramework.Framework45
                            ? $"set {{ SetValue(ref {fieldName}, value); }}"
                            : $"set {{ SetValue(ref {fieldName}, value, \"{column.PropertyName}\"); }}");
                }


                sb.AppendLine(Tab2, "}");
            }

            CreateModelValidation(table, sb);

            sb.AppendLine(Tab1, "}");
            sb.AppendLine("}");

            return sb;
        }

        private void CreateModelValidation(Table table, StringBuilder sb)
        {
            sb.AppendLine(Tab2, "public override List<ValidationError> Validate()");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var validationErrors = new List<ValidationError>();");
            sb.AppendLine("");

            foreach (var column in table.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    if (!column.IsNullable)
                    {
                        sb.AppendLine(Tab3, $"if (string.IsNullOrEmpty({column.PropertyName}))");
                        sb.AppendLine(Tab3,
                            _cSharpVersion >= CSharpVersion.CSharp6
                                ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot be null\"));"
                                : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot be null\"));");
                    }

                    sb.AppendLine(Tab3, $"if (!string.IsNullOrEmpty({column.PropertyName}) && {column.PropertyName}.Length > {column.MaxLength})");
                    sb.AppendLine(Tab3,
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Max length is {column.MaxLength}\"));"
                            : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Max length is {column.MaxLength}\"));");
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

                            sb.AppendLine(Tab3, $"if (Math.Floor({column.PropertyName}) > {maxValueForNum})");

                            sb.AppendLine(Tab3,
                                _cSharpVersion >= CSharpVersion.CSharp6
                                    ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot exceed {maxValueForNum}\"));"
                                    : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot exceed {maxValueForNum}\"));");
                            sb.AppendLine(Tab3, $"if (GetDecimalPlaces({column.PropertyName}) > {column.MaxDecimalLength})");

                            sb.AppendLine(Tab3,
                                _cSharpVersion >= CSharpVersion.CSharp6
                                    ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot have more than {column.MaxDecimalLength} decimal place{(column.MaxDecimalLength > 1 ? "s" : "")}\"));"
                                    : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot have more than {column.MaxDecimalLength} decimal place{(column.MaxDecimalLength > 1 ? "s" : "")}\"));");
                            break;
                        case DateTime _:
                            sb.AppendLine(Tab3, $"if ({column.PropertyName} == DateTime.MinValue)");

                            sb.AppendLine(Tab3,
                                _cSharpVersion >= CSharpVersion.CSharp6
                                    ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot be default.\"));"
                                    : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot be default.\"));");
                            break;
                        case TimeSpan _:
                            sb.AppendLine(Tab3, $"if ({column.PropertyName} == TimeSpan.MinValue)");

                            sb.AppendLine(Tab3,
                                _cSharpVersion >= CSharpVersion.CSharp6
                                    ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot be default.\"));"
                                    : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot be default.\"));");
                            break;
                        case DateTimeOffset _:
                            sb.AppendLine(Tab3, $"if ({column.PropertyName} == DateTimeOffset.MinValue)");

                            sb.AppendLine(Tab3,
                                _cSharpVersion >= CSharpVersion.CSharp6
                                    ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot be default.\"));"
                                    : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot be default.\"));");
                            break;
                        case Guid _:
                            sb.AppendLine(Tab3, $"if ({column.PropertyName} == Guid.Empty)");

                            sb.AppendLine(Tab3,
                                _cSharpVersion >= CSharpVersion.CSharp6
                                    ? $"validationErrors.Add(new ValidationError(nameof({column.PropertyName}), \"Value cannot be default.\"));"
                                    : $"validationErrors.Add(new ValidationError(\"{column.PropertyName}\", \"Value cannot be default.\"));");
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
            for (var i = 0; i < columns.Count; i += VARIABLE_BLOCK_SCOPE)
            {
                for (var j = 0; j < Math.Min(VARIABLE_BLOCK_SCOPE, columns.Count - i); j++)
                {
                    sb.Append($"item.{columns[i + j].PropertyName}");
                    if (columns[i + j] != columns.Last()) sb.Append(", ");
                }

                if (i + VARIABLE_BLOCK_SCOPE >= columns.Count)
                    continue;
                sb.AppendLine("");
                sb.Append(Tab4);
            }

            return sb;
        }

        #region Repo Generation 

        private StringBuilder Interface(Table table)
        {
            var sb = new StringBuilder();
            sb.AppendLine(Tab1, $"public partial interface I{table.ClassName}Repository : IBaseRepository<{table.ClassName}>");
            sb.AppendLine(Tab1, "{");
            if (table.HasCompositeKey)
            {
                var pkParamList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.DataType.Name} {column.FieldName}, ")
                    .TrimEnd(' ', ',');

                sb.AppendLine(Tab2, $"{table.ClassName} Get({pkParamList});");
                sb.AppendLine(Tab2, $"{table.ClassName} Get({table.ClassName}Keys compositeId);");
                sb.AppendLine(Tab2, $"IEnumerable<{table.ClassName}> Get(List<{table.ClassName}Keys> compositeIds);");
                sb.AppendLine(Tab2, $"IEnumerable<{table.ClassName}> Get(params {table.ClassName}Keys[] compositeIds);");
                sb.AppendLine("");
                sb.AppendLine(Tab2, $"bool Update({table.ClassName} item);");
                sb.AppendLine(Tab2, $"bool Delete({table.ClassName} item);");
                sb.AppendLine(Tab2, $"bool Delete({pkParamList});");
                sb.AppendLine(Tab2, $"bool Delete({table.ClassName}Keys compositeId);");
                sb.AppendLine("");
            }
            else if (table.PrimaryKeys.Any())
            {
                var pk = table.PrimaryKeys.First();
                sb.AppendLine(Tab2, $"{table.ClassName} Get({pk.DataType.Name} {pk.FieldName});");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(List<{pk.DataType.Name}> {pk.FieldName}s);");
                sb.AppendLine(Tab2,
                    $"IEnumerable<{table.ClassName}> Get(params {pk.DataType.Name}[] {pk.FieldName}s);");
                sb.AppendLine("");
                sb.AppendLine(Tab2, $"bool Update({table.ClassName} item);");
                sb.AppendLine(Tab2, $"bool Delete({table.ClassName} item);");
                sb.AppendLine(Tab2, $"bool Delete({pk.DataType.Name} {pk.FieldName});");
                sb.AppendLine("");
            }

            sb.AppendLine(Tab2, $"IEnumerable<{table.ClassName}> Search(");
            foreach (var column in table.Columns)
            {
                if (_cSharpVersion >= CSharpVersion.CSharp4)
                {
                    sb.Append(Tab3,
                        column.DataType != typeof(XmlDocument)
                            ? $"{column.DataType.Name}{(IsNullable(column.DataType.Name) ? "?" : string.Empty)} {column.FieldName} = null"
                            : $"String {column.FieldName} = null");
                }
                else
                {
                    sb.Append(Tab3,
                        column.DataType != typeof(XmlDocument)
                            ? $"{column.DataType.Name}{(IsNullable(column.DataType.Name) ? "?" : string.Empty)} {column.FieldName}"
                            : $"String {column.FieldName}");
                }

                sb.AppendLine(column == table.Columns.Last() ? ");" : ",");
            }

            if (table.HasCompositeKey)
            {
                sb.AppendLine("");
                //Find methods on PK'S are available as there's a composite primary key
                foreach (var primaryKey in table.PrimaryKeys)
                {
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{primaryKey.PropertyName}({primaryKey.DataType.Name} {primaryKey.FieldName});");
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{primaryKey.PropertyName}(FindComparison comparison, {primaryKey.DataType.Name} {primaryKey.FieldName});");
                }
            }

            sb.AppendLine("");

            foreach (var nonPrimaryKey in table.NonPrimaryKeys)
            {

                if (nonPrimaryKey.DataType != typeof(XmlDocument))
                {
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataType.Name} {nonPrimaryKey.FieldName});");
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataType.Name} {nonPrimaryKey.FieldName});");
                }
                else
                {
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName});");
                    sb.AppendLine(Tab2,
                        $"IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName});");
                }
            }

            if (table.PrimaryKeys.Any())
            {
                sb.AppendLine(Tab2, $"bool Merge(List<{table.ClassName}> items);");
            }

            sb.AppendLine(Tab1, "}");
            return sb;
        }

        private StringBuilder Repo_Get(Table table)
        {
            var sb = new StringBuilder();
            if (table.HasCompositeKey)
            {
                var pkParamList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.DataType.Name} {column.FieldName}, ")
                    .TrimEnd(' ', ',');

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public {table.ClassName} Get({pkParamList})");
                sb.AppendLine(Tab2, "{");
                sb.Append(Tab3, "return Where(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append(_cSharpVersion >= CSharpVersion.CSharp6
                        ? $"{(pk.DbColName == nameof(pk.DbColName) ? $"nameof({table.ClassName}.{pk.DbColName})" : $"\"{pk.DbColName}\"")}, Comparison.Equals, {pk.FieldName}"
                        : $"\"{pk.PropertyName}\", Comparison.Equals, {pk.FieldName}");
                    if (pk != table.PrimaryKeys.Last())
                    {
                        sb.Append(").And(");
                    }
                }

                sb.AppendLine(").Results().FirstOrDefault();");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public {table.ClassName} Get({table.ClassName}Keys compositeId)");
                sb.AppendLine(Tab2, "{");
                sb.Append(Tab3, "return Where(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append(_cSharpVersion >= CSharpVersion.CSharp6
                        ? $"{(pk.DbColName == nameof(pk.DbColName) ? $"nameof({table.ClassName}.{pk.DbColName})" : $"\"{pk.DbColName}\"")}, Comparison.Equals, compositeId.{pk.PropertyName}"
                        : $"\"{pk.PropertyName}\", Comparison.Equals, compositeId.{pk.PropertyName}");
                    if (pk != table.PrimaryKeys.Last())
                    {
                        sb.Append(").And(");
                    }
                }

                sb.AppendLine(").Results().FirstOrDefault();");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> Get(List<{table.ClassName}Keys> compositeIds)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, "return Get(compositeIds.ToArray());");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> Get(params {table.ClassName}Keys[] compositeIds)");
                sb.AppendLine(Tab2, "{");

                sb.Append(Tab3, "var result = Where(");
                foreach (var pk in table.PrimaryKeys)
                {
                    sb.Append(_cSharpVersion >= CSharpVersion.CSharp6
                        ? $"{(pk.DbColName == nameof(pk.DbColName) ? $"nameof({table.ClassName}.{pk.DbColName})" : $"\"{pk.DbColName}\"")}, Comparison.In, compositeIds.Select(x => x.{pk.PropertyName}).ToList()"
                        : $"\"{pk.PropertyName}\", Comparison.In, compositeIds.Select(x => x.{pk.PropertyName}).ToList()");
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
                    sb.Append($"x.{pk.DbColName} == compositeKey.{pk.DbColName}");
                    if (pk != table.PrimaryKeys.Last())
                        sb.Append(" && ");
                }
                sb.AppendLine("));");
                sb.AppendLine(Tab3, "}");
                sb.AppendLine(Tab3, "return filteredResults;");

                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
            }
            else if (table.PrimaryKeys.Any())
            {
                var pk = table.PrimaryKeys.First();

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public {table.ClassName} Get({pk.DataType.Name} {pk.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3,
                    _cSharpVersion >= CSharpVersion.CSharp6
                        ? $"return Where({(pk.DbColName == nameof(pk.DbColName) ? $"nameof({table.ClassName}.{pk.DbColName})" : $"\"{pk.DbColName}\"")}, Comparison.Equals, {pk.FieldName}).Results().FirstOrDefault();"
                        : $"return Where(\"{pk.PropertyName}\", Comparison.Equals, {pk.FieldName}).Results().FirstOrDefault();");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    $"public IEnumerable<{table.ClassName}> Get(List<{pk.DataType.Name}> {pk.FieldName}s)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"return Get({pk.FieldName}s.ToArray());");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> Get(params {pk.DataType.Name}[] {pk.FieldName}s)");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3,
                    _cSharpVersion >= CSharpVersion.CSharp6
                        ? $"return Where({(pk.DbColName == nameof(pk.DbColName) ? $"nameof({table.ClassName}.{pk.DbColName})" : $"\"{pk.DbColName}\"")}, Comparison.In, {pk.FieldName}s).Results();"
                        : $"return Where(\"{pk.PropertyName}\", Comparison.In, {pk.FieldName}s).Results();");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
            }

            return sb;
        }

        private StringBuilder Repo_Create(Table table)
        {
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

            sb.Append(Tab3, "var createdKeys = BaseCreate(");
            sb.Append(PrintBlockScopedVariables(table.Columns));
            sb.AppendLine(");");

            sb.AppendLine(Tab3, "if (createdKeys.Count != Columns.Count(x => x.PrimaryKey))");
            sb.AppendLine(Tab4, "return false;");
            sb.AppendLine("");
            foreach (var pk in table.PrimaryKeys)
            {
                sb.AppendLine(Tab3,
                    _cSharpVersion >= CSharpVersion.CSharp6
                        ? $"item.{pk.PropertyName} = ({pk.DataType.Name})createdKeys[nameof({table.ClassName}.{pk.PropertyName})];"
                        : $"item.{pk.PropertyName} = ({pk.DataType.Name})createdKeys[\"{pk.PropertyName}\"];");
            }

            sb.AppendLine(Tab3, "item.ResetDirty();");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "return true;");
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
            sb.AppendLine(Tab3, "var dt = new DataTable();");
            sb.AppendLine(Tab3, "foreach (var mergeColumn in Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.Identity))");
            sb.AppendLine(Tab4, "dt.Columns.Add(mergeColumn.ColumnName);");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "foreach (var item in items)");
            sb.AppendLine(Tab3, "{");


            sb.Append(Tab4, "dt.Rows.Add(");
            sb.Append(PrintBlockScopedVariables(table.Columns.Where(x => !x.PrimaryKey || x.PrimaryKey && !x.IsIdentity).ToList()));
            sb.AppendLine("); ");
            sb.AppendLine(Tab3, "}");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "return BulkInsert(dt);");
            sb.AppendLine(Tab2, "}");

            sb.AppendLine(Tab2, $"public override bool BulkCreate(List<{table.ClassName}> items)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "return BulkCreate(items.ToArray());");
            sb.AppendLine(Tab2, "}");


            return sb;
        }

        private StringBuilder Repo_Update(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public bool Update({table.ClassName} item)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "if (item == null)");
            sb.AppendLine(Tab4, "return false;");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "var validationErrors = item.Validate();");
            sb.AppendLine(Tab3, "if (validationErrors.Any())");
            sb.AppendLine(Tab4, "throw new ValidationException(validationErrors);");
            sb.AppendLine("");

            sb.AppendLine(Tab3, "var success = BaseUpdate(item.DirtyColumns, ");
            sb.Append(Tab4);
            sb.Append(PrintBlockScopedVariables(table.Columns));

            sb.AppendLine(");");
            sb.AppendLine("");

            sb.AppendLine(Tab3, "if (success)");
            sb.AppendLine(Tab3, "item.ResetDirty();");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "return success;");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private StringBuilder Repo_Delete(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine(Tab2, $"public bool Delete({table.ClassName} {table.LowerClassName})");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $"if ({table.LowerClassName} == null)");
            sb.AppendLine(Tab4, "return false;");
            sb.AppendLine("");

            sb.AppendLine(Tab3, "var deleteTable = new DeleteTable();");
            foreach (var pk in table.PrimaryKeys)
            {
                sb.AppendLine(Tab3,
                    _cSharpVersion >= CSharpVersion.CSharp6
                        ? $"deleteTable.AddColumn({(pk.DbColName == nameof(pk.DbColName) ? $"nameof({table.ClassName}.{pk.DbColName})" : $"\"{pk.DbColName}\"")}, {table.LowerClassName}.{pk.PropertyName});"
                        : $"deleteTable.AddColumn(\"{pk.PropertyName}\", {table.LowerClassName}.{pk.PropertyName});");
            }

            sb.AppendLine("");
            sb.AppendLine(Tab3, "return BaseDelete(deleteTable);");
            sb.AppendLine(Tab2, "}");

            if (table.HasCompositeKey)
            {
                var pkParamList = table.PrimaryKeys.Aggregate("",
                        (current, column) => current + $"{column.DataType.Name} {column.FieldName}, ")
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
            }
            else if (table.PrimaryKeys.Any())
            {
                var pk = table.PrimaryKeys.First();

                sb.AppendLine("");
                sb.AppendLine(Tab2, $"public bool Delete({pk.DataType.Name} {pk.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3,
                    $"return Delete(new {table.ClassName} {{ {pk.PropertyName} = {pk.FieldName} }});");
                sb.AppendLine(Tab2, "}");
                sb.AppendLine("");
            }

            return sb;
        }

        private StringBuilder Repo_Merge(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public bool Merge(List<{table.ClassName}> items)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var mergeTable = new List<object[]>();");

            sb.AppendLine(Tab3, "foreach (var item in items)");
            sb.AppendLine(Tab3, "{");
            sb.AppendLine(Tab4, "mergeTable.Add(new object[]");
            sb.AppendLine(Tab4, "{");

            foreach (var column in table.Columns)
            {
                if (column.PrimaryKey)
                    sb.Append(Tab5, $"item.{column.PropertyName}");
                else
                {
                    sb.Append(Tab5,
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"item.{column.PropertyName}, item.DirtyColumns.Contains({(column.DbColName == nameof(column.DbColName) ? $"nameof({table.ClassName}.{column.DbColName})" : $"\"{column.DbColName}\"")})"
                            : $"item.{column.PropertyName}, item.DirtyColumns.Contains(\"{column.DbColName}\")");
                }

                sb.AppendLine(column != table.Columns.Last() ? "," : "");
            }

            sb.AppendLine(Tab4, "});");
            sb.AppendLine(Tab3, "}");

            sb.AppendLine(Tab3, "return BaseMerge(mergeTable);");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private StringBuilder Repo_ToItem(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"protected override {table.ClassName} ToItem(DataRow row)");
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, $" var item = new {table.ClassName}");
            sb.AppendLine(Tab3, "{");

            foreach (var column in table.Columns)
            {
                sb.AppendLine(Tab4,
                    _cSharpVersion >= CSharpVersion.CSharp6
                        ? $"{column.PropertyName} = Get{column.DataType.Name}(row, {(column.DbColName == nameof(column.DbColName) ? $"nameof({table.ClassName}.{column.DbColName})" : $"\"{column.DbColName}\"")}),"
                        : $"{column.PropertyName} = Get{column.DataType.Name}(row, \"{column.DbColName}\"),");
            }
            sb.AppendLine(Tab3, "};");
            sb.AppendLine("");
            sb.AppendLine(Tab3, "item.ResetDirty();");
            sb.AppendLine(Tab3, "return item;");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private StringBuilder Repo_Search(Table table)
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> Search(");

            foreach (var column in table.Columns)
            {
                if (_cSharpVersion >= CSharpVersion.CSharp4)
                {
                    sb.Append(Tab3,
                        column.DataType != typeof(XmlDocument)
                            ? $"{column.DataType.Name}{(IsNullable(column.DataType.Name) ? "?" : string.Empty)} {column.FieldName} = null"
                            : $"String {column.FieldName} = null");
                }
                else
                {
                    sb.Append(Tab3,
                        column.DataType != typeof(XmlDocument)
                            ? $"{column.DataType.Name}{(IsNullable(column.DataType.Name) ? "?" : string.Empty)} {column.FieldName}"
                            : $"String {column.FieldName}");
                }

                sb.AppendLine(column == table.Columns.Last() ? ")" : ",");
            }
            sb.AppendLine(Tab2, "{");
            sb.AppendLine(Tab3, "var queries = new List<QueryItem>(); ");
            sb.AppendLine("");
            foreach (var column in table.Columns)
            {
                if (IsNullable(column.DataType.Name))
                {
                    sb.AppendLine(Tab3, $"if ({column.FieldName}.HasValue)");
                }
                else switch (column.DataType.Name)
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
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"queries.Add(new QueryItem({(column.DbColName == nameof(column.DbColName) ? $"nameof({table.ClassName}.{column.DbColName})" : $"\"{column.DbColName}\"")}, {column.FieldName}));"
                            : $"queries.Add(new QueryItem(\"{column.DbColName}\", {column.FieldName}));");
                }
                else
                {
                    sb.AppendLine(Tab4,
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"queries.Add(new QueryItem({(column.DbColName == nameof(column.DbColName) ? $"nameof({table.ClassName}.{column.DbColName})" : $"\"{column.DbColName}\"")}, {column.FieldName}, typeof(XmlDocument)));"
                            : $"queries.Add(new QueryItem(\"{column.DbColName}\", {column.FieldName}, typeof(XmlDocument)));");
                }
            }

            sb.AppendLine("");
            sb.AppendLine(Tab3, "return BaseSearch(queries);");
            sb.AppendLine(Tab2, "}");

            return sb;
        }

        private StringBuilder Repo_Find(Table table)
        {
            var sb = new StringBuilder();

            if (table.HasCompositeKey)
            {
                sb.AppendLine("");
                //Find methods on PK'S are available as there's a composite primary key
                foreach (var primaryKey in table.PrimaryKeys)
                {
                    sb.AppendLine("");
                    sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> FindBy{primaryKey.DbColName}({primaryKey.DataType.Name} {primaryKey.FieldName})");
                    sb.AppendLine(Tab2, "{");
                    sb.AppendLine(Tab3, $"return FindBy{primaryKey.PropertyName}(FindComparison.Equals, {primaryKey.FieldName});");
                    sb.AppendLine(Tab2, "}");

                    sb.AppendLine("");
                    sb.AppendLine(Tab2, $"public IEnumerable<{table.ClassName}> FindBy{primaryKey.DbColName}(FindComparison comparison, {primaryKey.DataType.Name} {primaryKey.FieldName})");
                    sb.AppendLine(Tab2, "{");

                    sb.AppendLine(Tab3,
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"return Where({(primaryKey.PropertyName == nameof(primaryKey.PropertyName) ? $"nameof({table.ClassName}.{primaryKey.PropertyName})" : $"\"{primaryKey.PropertyName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {primaryKey.FieldName}).Results();"
                            : $"return Where(\"{primaryKey.PropertyName}\", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {primaryKey.FieldName}).Results();");
                    sb.AppendLine(Tab2, "}");
                }
            }

            foreach (var nonPrimaryKey in table.NonPrimaryKeys)
            {
                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}({nonPrimaryKey.DataType.Name} {nonPrimaryKey.FieldName})"
                        : $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(String {nonPrimaryKey.FieldName})");
                sb.AppendLine(Tab2, "{");
                sb.AppendLine(Tab3, $"return FindBy{nonPrimaryKey.PropertyName}(FindComparison.Equals, {nonPrimaryKey.FieldName});");
                sb.AppendLine(Tab2, "}");

                sb.AppendLine("");
                sb.AppendLine(Tab2,
                    nonPrimaryKey.DataType != typeof(XmlDocument)
                        ? $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, {nonPrimaryKey.DataType.Name} {nonPrimaryKey.FieldName})"
                        : $"public IEnumerable<{table.ClassName}> FindBy{nonPrimaryKey.PropertyName}(FindComparison comparison, String {nonPrimaryKey.FieldName})");
                sb.AppendLine(Tab2, "{");
                if (nonPrimaryKey.DataType != typeof(XmlDocument))
                {
                    sb.AppendLine(Tab3,
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"return Where({(nonPrimaryKey.DbColName == nameof(nonPrimaryKey.DbColName) ? $"nameof({table.ClassName}.{nonPrimaryKey.DbColName})" : $"\"{nonPrimaryKey.DbColName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {nonPrimaryKey.FieldName}).Results();"
                            : $"return Where(\"{nonPrimaryKey.DbColName}\", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {nonPrimaryKey.FieldName}).Results();");
                }
                else
                {
                    sb.AppendLine(Tab3,
                        _cSharpVersion >= CSharpVersion.CSharp6
                            ? $"return Where({(nonPrimaryKey.DbColName == nameof(nonPrimaryKey.DbColName) ? $"nameof({table.ClassName}.{nonPrimaryKey.DbColName})" : $"\"{nonPrimaryKey.DbColName}\"")}, (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {nonPrimaryKey.FieldName}, typeof(XmlDocument)).Results();"
                            : $"return Where(\"{nonPrimaryKey.DbColName}\", (Comparison)Enum.Parse(typeof(Comparison), comparison.ToString()), {nonPrimaryKey.FieldName}, typeof(XmlDocument)).Results();");
                }
                sb.AppendLine(Tab2, "}");
            }

            return sb;
        }

        #endregion

        #endregion
    }
}
