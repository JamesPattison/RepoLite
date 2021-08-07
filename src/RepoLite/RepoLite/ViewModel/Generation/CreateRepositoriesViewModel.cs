﻿using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RepoLite.Commands;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.Common.Options;
using RepoLite.DataAccess;
using RepoLite.GeneratorEngine;
using RepoLite.GeneratorEngine.Models;
using RepoLite.ViewModel.Base;
using RepoLite.Views;
using RepoLite.Views.Generation;

namespace RepoLite.ViewModel.Generation
{
    public class CreateRepositoriesViewModel : ViewModelBase
    {
        private bool _loaded;
        private GenerationOptions _generationSettings;
        private SystemOptions _systemSettings;
        private IDataSource _dataSource;
        private IGenerator _generator;
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<EntityToGenerate> Tables { get; set; } = new ObservableCollection<EntityToGenerate>();

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
                    foreach (var table in tableDefinitions.OrderBy(x => x.Schema).ThenBy(x => x.Name).ToList())
                    {
                        Tables.Add(new EntityToGenerate { Schema = table.Schema, Table = table.Name });
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

                    var tableDefinitions = _dataSource
                        .LoadTables(tables.Select(x => new NameAndSchema(x.Schema, x.Table)).ToList()).ToList();

                    DoWork(() =>
                    {
                        var createModelViewModel = new CreateModelsViewModel();
                        CreateBaseRepository(_generator);
                        if (true)
                        {
                            createModelViewModel.CreateBaseModel(_generator);
                        }

                        tableDefinitions.ForEach(x =>
                        {
                            var model = _generator.BuildModel(new RepositoryGenerationObject(x, tableDefinitions));

                            createModelViewModel.CreateModel(x, _generator, model);
                                
                            LogMessage($"Processing Table {x.Schema}.{x.ClassName}");
                            var repository = _generator.BuildRepository(new RepositoryGenerationObject(x, tableDefinitions));

                            CreateRepo(x, _generator, repository);
                        });
                    }, () =>
                    {
                        
                        Process.Start("explorer.exe", _generationSettings.OutputDirectory);
                    });
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

        public CreateRepositoriesViewModel()
        {
            _generationSettings = IOC.Resolve<IOptions<GenerationOptions>>().Value;
            _systemSettings = IOC.Resolve<IOptions<SystemOptions>>().Value;
            _dataSource = IOC.Resolve<DataSourceResolver>().Invoke(_systemSettings.DataSource);
            _generator = IOC.Resolve<GeneratorResolver>().Invoke(_systemSettings.DataSource, _systemSettings.GenerationLanguage);
        }
        
        internal void CreateRepo(Table table, IGenerator generator, string repositoryName)
        {
            var outputDirectory = $"{_generationSettings.OutputDirectory}/Repositories";

            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            var result = table.RepositoryName;

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
            LogMessage($"Creating Repository File for {table.Schema}.{table.ClassName} in {outputDirectory}/");
            File.WriteAllText(fileName, repositoryName);
            LogMessage($"Done {table.Schema}.{table.ClassName}!");
        }

        internal void CreateBaseRepository(IGenerator generator)
        {
            var outputDirectory = $"{_generationSettings.OutputDirectory}/Repositories";

            if (!Directory.Exists($"{outputDirectory}/Base"))
                Directory.CreateDirectory($"{outputDirectory}/Base");

            //Write base repository
            var baseRepo = _generator.BuildBaseRepository();
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