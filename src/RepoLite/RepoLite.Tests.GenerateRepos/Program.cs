using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.DataAccess;
using RepoLite.GeneratorEngine;
using RepoLite.GeneratorEngine.Generators.BaseParsers;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RepoLite.Tests.GenerateRepos
{
    class Program
    {
        private static TargetFramework _originalTarget;
        private static CSharpVersion _originalCSharpVerison;
        private static GenerationLanguage _originalGenerationLanguage;
        private static string _originalOutputDir;
        private static string _originalModelFormat;
        private static string _originalRepoFormat;

        static void Main(string[] args)
        {
            //For setting them back after generating repos
            _originalTarget = AppSettings.Generation.TargetFramework;
            _originalCSharpVerison = AppSettings.Generation.CSharpVersion;
            _originalGenerationLanguage = AppSettings.System.GenerationLanguage;
            _originalOutputDir = AppSettings.Generation.OutputDirectory;
            _originalModelFormat = AppSettings.Generation.ModelFileNameFormat;
            _originalRepoFormat = AppSettings.Generation.RepositoryFileNameFormat;

            var combosToGen = new Dictionary<CSharpVersion, List<TargetFramework>>
            {
                {CSharpVersion.CSharp4, new List<TargetFramework>{TargetFramework.Framework35, TargetFramework.Framework45, TargetFramework.Framework471}},
                {CSharpVersion.CSharp6, new List<TargetFramework>{TargetFramework.Framework35, TargetFramework.Framework45, TargetFramework.Framework471}},
                {CSharpVersion.CSharp72, new List<TargetFramework>{TargetFramework.Framework35, TargetFramework.Framework45, TargetFramework.Framework471}}
            };

            var dataSource = DataSource.GetDataSource();
            var tableDefinitions = dataSource.LoadTables(dataSource.GetTables().Select(x => new TableAndSchema(x.Schema, x.Table)).ToList());

            AppSettings.Generation.OutputDirectory = "../../../RepoLite.Tests/ActualGeneratedFilesTests/GeneratedFiles";

            var generator = CodeGenerator.GetGenerator();

            CreateBaseModel(AppSettings.Generation.OutputDirectory, generator);
            CreateBaseRepository(AppSettings.Generation.OutputDirectory, generator);

            foreach (var table in tableDefinitions)
            {
                foreach (var csharpVer in combosToGen)
                {
                    AppSettings.Generation.CSharpVersion = csharpVer.Key;
                    foreach (var framework in csharpVer.Value)
                    {
                        AppSettings.Generation.TargetFramework = framework;

                        AppSettings.Generation.ModelGenerationNamespace = $"m.{table.ClassName}.{csharpVer.Key}.{framework}";
                        AppSettings.Generation.RepositoryGenerationNamespace = $"{table.ClassName}.{csharpVer.Key}.{framework}";

                        AppSettings.Generation.OutputDirectory = $"../../../RepoLite.Tests/ActualGeneratedFilesTests/GeneratedFiles/{csharpVer.Key}/{framework}";

                        var model = generator.ModelForTable(table).ToString();

                        var result = Regex.Replace(
                            AppSettings.Generation.ModelFileNameFormat,
                            Regex.Escape("{Name}"),
                            table.ClassName.Replace("$", "$$"),
                            RegexOptions.IgnoreCase
                        );

                        result = Regex.Replace(
                            result,
                            Regex.Escape("{Schema}"),
                            table.Schema.Replace("$", "$$"),
                            RegexOptions.IgnoreCase
                        );

                        var fileName = "";

                        if (AppSettings.System.GenerationLanguage == GenerationLanguage.CSharp)
                            fileName = $"{AppSettings.Generation.OutputDirectory}/{result}.{generator.FileExtension()}";

                        if (!Directory.Exists(AppSettings.Generation.OutputDirectory)) Directory.CreateDirectory(AppSettings.Generation.OutputDirectory);
                        File.WriteAllText(fileName, model);

                        var repository = generator.RepositoryForTable(table).ToString();

                        result = Regex.Replace(
                            AppSettings.Generation.RepositoryFileNameFormat,
                            Regex.Escape("{Name}"),
                            table.ClassName.Replace("$", "$$"),
                            RegexOptions.IgnoreCase
                        );

                        result = Regex.Replace(
                            result,
                            Regex.Escape("{Schema}"),
                            table.Schema.Replace("$", "$$"),
                            RegexOptions.IgnoreCase
                        );

                        if (AppSettings.System.GenerationLanguage == GenerationLanguage.CSharp)
                            fileName = $"{AppSettings.Generation.OutputDirectory}/{result}.{generator.FileExtension()}";

                        if (!Directory.Exists(AppSettings.Generation.OutputDirectory)) Directory.CreateDirectory(AppSettings.Generation.OutputDirectory);
                        File.WriteAllText(fileName, repository);
                    }
                }
            }

            AppSettings.Generation.TargetFramework = _originalTarget;
            AppSettings.Generation.CSharpVersion = _originalCSharpVerison;
            AppSettings.System.GenerationLanguage = _originalGenerationLanguage;
            AppSettings.Generation.OutputDirectory = _originalOutputDir;
            AppSettings.Generation.ModelFileNameFormat = _originalModelFormat;
            AppSettings.Generation.RepositoryFileNameFormat = _originalRepoFormat;
        }

        private static void CreateBaseModel(string outputDirectory, IGenerator generator)
        {
            if (!Directory.Exists($"{outputDirectory}/Base"))
                Directory.CreateDirectory($"{outputDirectory}/Base");

            BaseClassParser parser;

            switch (AppSettings.System.GenerationLanguage)
            {
                case GenerationLanguage.CSharp:
                    parser = new CSharpSqlServerBaseClassParser(AppSettings.Generation.TargetFramework,
                        AppSettings.Generation.CSharpVersion);
                    break;
                default:
                    //"If you've added a new language to the enum, the generator needs creating and hooking up here"
                    throw new ArgumentOutOfRangeException();
            }

            //Write base model
            var baseModel = parser.BuildBaseModel();
            File.WriteAllText($"{outputDirectory}/Base/BaseModel.{generator.FileExtension()}", baseModel);
        }

        private static void CreateBaseRepository(string outputDirectory, IGenerator generator)
        {
            if (!Directory.Exists($"{outputDirectory}/Base"))
                Directory.CreateDirectory($"{outputDirectory}/Base");

            BaseClassParser parser;

            switch (AppSettings.System.GenerationLanguage)
            {
                case GenerationLanguage.CSharp:
                    parser = new CSharpSqlServerBaseClassParser(AppSettings.Generation.TargetFramework,
                        AppSettings.Generation.CSharpVersion);
                    break;
                default:
                    //"If you've added a new language to the enum, the generator needs creating and hooking up here"
                    throw new ArgumentOutOfRangeException();
            }

            //Write base repository
            var baseRepo = parser.BuildBaseRepository();
            File.WriteAllText($"{outputDirectory}/Base/BaseRepository.{generator.FileExtension()}", baseRepo);
        }
    }
}
