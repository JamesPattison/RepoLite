using RepoLite.Commands;
using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.DataAccess;
using RepoLite.GeneratorEngine;
using RepoLite.GeneratorEngine.Generators.BaseParsers;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using RepoLite.GeneratorEngine.Models;
using RepoLite.ViewModel.Base;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace RepoLite.ViewModel.Main
{
    public class CreateModelsViewModel : ViewModelBase
    {
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<TableToGenerate> Tables { get; set; } = new ObservableCollection<TableToGenerate>();

        public ICommand LoadTables
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Tables.Clear();
                    var dataSource = DataSource.GetDataSource();
                    var tableDefinitions = dataSource.GetTables();
                    foreach (var table in tableDefinitions.OrderBy(x => x.Schema).ThenBy(x => x.Table).ToList())
                    {
                        Tables.Add(new TableToGenerate { Schema = table.Schema, Table = table.Table });
                    }
                    OnPropertyChanged(nameof(Tables));
                });
            }
        }

        public ICommand Generate
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var tables = Tables.Where(x => x.Selected);

                    var dataSource = DataSource.GetDataSource();
                    var tableDefinitions = dataSource.LoadTables(tables.Select(x => new TableAndSchema(x.Schema, x.Table)).ToList());

                    var generator = CodeGenerator.GetGenerator();
                    var outputDirectory = AppSettings.Generation.OutputDirectory.TrimEnd('/').TrimEnd('\\');

                    if (!Directory.Exists(outputDirectory))
                        Directory.CreateDirectory(outputDirectory);

                    DoWork(() =>
                    {
                        CreateBaseModel(outputDirectory, generator);

                        tableDefinitions.ForEach(x =>
                        {
                            LogMessage($"Processing Table {x.Schema}.{x.ClassName}");
                            var model = generator.ModelForTable(x).ToString();

                            CreateModel(x, outputDirectory, generator, model);
                        });
                    }, () => Process.Start(outputDirectory));
                });
            }
        }

        internal void CreateModel(Table table, string outputDirectory, IGenerator generator, string modelName)
        {
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

            string fileName;

            switch (AppSettings.System.GenerationLanguage)
            {
                case GenerationLanguage.CSharp:
                    fileName = $"{outputDirectory}/{result}.{generator.FileExtension()}";
                    break;
                default:
                    //"If you've added a new language to the enum, the generator needs creating and hooking up here"
                    throw new ArgumentOutOfRangeException();
            }

            if (!Directory.Exists(AppSettings.Generation.OutputDirectory))
                Directory.CreateDirectory(AppSettings.Generation.OutputDirectory);
            LogMessage($"Creating Model File for {table.Schema}.{table.ClassName} in {outputDirectory}/");
            File.WriteAllText(fileName, modelName);
            LogMessage($"Done {table.Schema}.{table.ClassName}!");
        }

        internal void CreateBaseModel(string outputDirectory, IGenerator generator)
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
            LogMessage($"Creating Base Model file in {outputDirectory}/Base/");
            File.WriteAllText($"{outputDirectory}/Base/BaseModel.{generator.FileExtension()}", baseModel);
            LogMessage("Created Base Model file.");
        }

        private void LogMessage(string message)
        {
            AddToList(message, Messages);
        }
    }
}
