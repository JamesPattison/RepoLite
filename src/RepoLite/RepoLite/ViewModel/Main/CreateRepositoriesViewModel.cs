using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RepoLite.Commands;
using RepoLite.Common.Enums;
using RepoLite.Common.Extensions;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;
using RepoLite.GeneratorEngine.Models;
using RepoLite.ViewModel.Base;
using RepoLite.Views;
using RepoLite.Views.Generation;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace RepoLite.ViewModel.Main
{
    public class CreateRepositoriesViewModel : ViewModelBase
    {
        private IDataSource _dataSource;
        private IGenerator _generator;
        private ITemplateParser _templateParser;
        private readonly GenerationSettings _generationSettings;
        private readonly SystemSettings _systemSettings;
        private readonly CreateModelsViewModel _createModelsViewModel;

        private bool _loaded;
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<TableToGenerate> Tables { get; set; } = new ObservableCollection<TableToGenerate>();

        public CreateRepositoriesViewModel(
            DataSourceResolver dataSourceResolver,
            GeneratorResolver generatorResolver,
            TemplateParserResolver templateParserResolver,
            IOptions<SystemSettings> systemSettings,
            IOptions<GenerationSettings> generationSettings,
            CreateModelsViewModel createModelsViewModel)
        {
            _dataSource = dataSourceResolver();
            _generator = generatorResolver();
            _templateParser = templateParserResolver();
            _systemSettings = systemSettings.Value;
            _generationSettings = generationSettings.Value;
            _createModelsViewModel = createModelsViewModel;
        }

        public bool Loaded
        {
            get => _loaded;
            set => SetProperty(ref _loaded, value);
        }

        public ICommand LoadTables
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Tables.Clear();
                    var tableDefinitions = _dataSource.GetTables();
                    foreach (var table in tableDefinitions.OrderBy(x => x.Schema).ThenBy(x => x.Table).ToList())
                    {
                        Tables.Add(new TableToGenerate { Schema = table.Schema, Table = table.Table });
                    }
                    OnPropertyChanged(nameof(Tables));
                    Loaded = true;
                });
            }
        }

        public ICommand ToggleSelectAll
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var shouldSelect = Math.Abs(Math.Round(Tables.Count(x => x.Selected) / (float)Tables.Count)) < 0.001;
                    foreach (var table in Tables)
                    {
                        table.Selected = shouldSelect;
                    }
                }, o => Loaded);
            }
        }

        public ICommand Generate
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var tables = Tables.Where(x => x.Selected);

                    var tableDefinitions = _dataSource.LoadTables(tables.Select(x => new TableAndSchema(x.Schema, x.Table)).ToList());

                    //var outputDirectory = AppSettings.GenerationSettings.OutputDirectory.TrimEnd('/').TrimEnd('\\');

                    //if (!Directory.Exists(outputDirectory))
                    //    Directory.CreateDirectory(outputDirectory);

                    DoWork(() =>
                    {
                        CreateBaseRepository(_generator);
                        if (true)
                        {
                            _createModelsViewModel.CreateBaseModel(_generator);
                        }

                        tableDefinitions.ForEach(x =>
                        {
                            if (true)
                            {
                                var model = _generator.ModelForTable(x, tableDefinitions).ToString();

                                _createModelsViewModel.CreateModel(x, _generator, model);
                            }

                            LogMessage($"Processing Table {x.Schema}.{x.DbTableName.ToModelName(_generationSettings.ModelClassNameFormat)}");
                            var repository = _generator.RepositoryForTable(x, tableDefinitions).ToString();

                            CreateRepo(x, _generator, repository);
                        });
                    }, () => Process.Start(_generationSettings.OutputDirectory));
                });
            }
        }

        public ICommand SaveSelection
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var dlg = new InputDialog();
                    dlg.ShowDialog();
                    var f = dlg.Value;

                    if (!Directory.Exists($"{App.ClientDataPath}TableSelections"))
                        Directory.CreateDirectory($"{App.ClientDataPath}TableSelections");

                    var presets = new DirectoryInfo($"{App.ClientDataPath}TableSelections").GetFiles();

                    if (presets.All(x => x.Name.Split('.')[0] != f))
                    {
                        var tableSelection = new TableSelection
                        {
                            Name = f,
                            SelectedTables = Tables.Where(x => x.Selected).Select(x => $"{x.Schema}.{x.Table}").ToList()
                        };

                        var json = JsonConvert.SerializeObject(tableSelection);

                        File.WriteAllText($@"{App.ClientDataPath}TableSelections\{f}.json", json);
                    }
                    else
                    {
                        LogMessage("Template already exists");
                    }
                });
            }
        }

        public ICommand LoadSelection
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var dlg = new LoadTemplates();
                    dlg.ShowDialog();
                    var f = dlg.SelectedItem;

                    if (!Directory.Exists($"{App.ClientDataPath}TableSelections"))
                        Directory.CreateDirectory($"{App.ClientDataPath}TableSelections");

                    var presets = new DirectoryInfo($"{App.ClientDataPath}TableSelections").GetFiles();

                    var template = presets.FirstOrDefault(x => x.Name.Split('.')[0] == f);
                    if (template == null) return;

                    var obj = JsonConvert.DeserializeObject<TableSelection>(File.ReadAllText(template.FullName));

                    foreach (var objSelectedTable in obj.SelectedTables)
                    {
                        var table = Tables.FirstOrDefault(x => $"{x.Schema}.{x.Table}" == objSelectedTable);
                        if (table != null)
                            table.Selected = true;
                    }
                });
            }
        }

        internal void CreateRepo(Table table, IGenerator generator, string repositoryName)
        {
            var outputDirectory = $"{_generationSettings.OutputDirectory}/Repositories";

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var result = table.DbTableName.ToRepositoryName(_generationSettings.RepositoryClassNameFormat);

            string fileName;

            switch (_systemSettings.GenerationLanguage)
            {
                case GenerationLanguage.CSharp:
                    fileName = $"{outputDirectory}/{result}.{generator.FileExtension()}";
                    break;
                default:
                    //"If you've added a new language to the enum, the generator needs creating and hooking up here"
                    throw new ArgumentOutOfRangeException();
            }
            LogMessage($"Creating Repository File for {table.Schema}.{table.DbTableName.ToModelName(_generationSettings.ModelClassNameFormat)} in {outputDirectory}/");
            File.WriteAllText(fileName, repositoryName);
            LogMessage($"Done {table.Schema}.{table.DbTableName.ToModelName(_generationSettings.ModelClassNameFormat)}!");
        }

        internal void CreateBaseRepository(IGenerator generator)
        {
            var outputDirectory = $"{_generationSettings.OutputDirectory}/Repositories";

            if (!Directory.Exists($"{outputDirectory}/Base"))
                Directory.CreateDirectory($"{outputDirectory}/Base");

            //Write base repository
            var baseRepo = _templateParser.BuildBaseRepository();
            LogMessage($"Creating Base Repository file in {outputDirectory}/Base/");
            File.WriteAllText($"{outputDirectory}/Base/BaseRepository.{generator.FileExtension()}", baseRepo);
            LogMessage("Created Base Repository file.");
        }

        private void LogMessage(string message)
        {
            AddToList(message, Messages);
        }
    }
}
