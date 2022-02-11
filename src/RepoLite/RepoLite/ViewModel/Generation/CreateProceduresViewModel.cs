using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using FirstFloor.ModernUI.Windows.Navigation;
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
using RepoLite.ViewModel.Generation.Procedures;
using RepoLite.Views;
using RepoLite.Views.Generation;
using RepoLite.Views.Generation.Procedures;

namespace RepoLite.ViewModel.Generation
{
    public class CreateProceduresViewModel : ViewModelBase
    {
        private bool _loaded;
        private GenerationOptions _generationSettings;
        private SystemOptions _systemSettings;
        private IDataSource _dataSource;
        private IGenerator _generator;
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<EntityToGenerate> Procedures { get; set; } = new ObservableCollection<EntityToGenerate>();

        public bool Loaded
        {
            get => _loaded;
            set => SetProperty(ref _loaded, value);
        }

        //public ObservableCollection<Procedure> SelectedProcedures => Procedures.Where(x => x.Selected).Select(x => x.)

        public ICommand LoadProcedures
        {
            get
            {
                return new RelayCommand(o =>
                {
                    Procedures.Clear();
                    var procedures = _dataSource.GetProcedures();
                    foreach (var procedure in procedures.OrderBy(x => x.Schema).ThenBy(x => x.Name).ToList())
                    {
                        Procedures.Add(new EntityToGenerate { Schema = procedure.Schema, Table = procedure.Name });
                    }
                    OnPropertyChanged(nameof(Procedures));
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
                    var shouldSelect = Math.Abs(Math.Round(Procedures.Count(x => x.Selected) / (float)Procedures.Count)) < 0.001;
                    foreach (var procedure in Procedures)
                    {
                        procedure.Selected = shouldSelect;
                    }
                }, o => Loaded);
            }
        }

        public ICommand Configure
        {
            get
            {
                return new RelayCommand(o =>
                {
                    var procedures = Procedures.Where(x => x.Selected);

                    var procedureDefinitions = _dataSource
                        .LoadProcedures(procedures.Select(x => new NameAndSchema(x.Schema, x.Table)).ToList()).ToList();

                    var procedureGenerationSettings = GetGenerationSettings();

                    var configureProcedures = new ConfigureProcedures
                    {
                        DataContext = new ConfigureProceduresViewModel(procedureDefinitions, procedureGenerationSettings)
                    };
                    configureProcedures.ShowDialog();
                });
            }
        }

        private Dictionary<string, List<string>> GetGenerationSettings()
        {
            var procedureResultsSetNamings = Properties.Settings.Default["PreviousProcedureResultSetNames"];
            var procedureGenerationSettings = new Dictionary<string, List<string>>();

            if (procedureResultsSetNamings != null &&
                procedureResultsSetNamings is string s &&
                !string.IsNullOrEmpty(s))
                procedureGenerationSettings = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(s);
            return procedureGenerationSettings;
        }

        public ICommand Generate
        {
            get
            {
                return new RelayCommand(o =>
                {

                    var procedures = Procedures.Where(x => x.Selected);

                    var procedureDefinitions = _dataSource
                        .LoadProcedures(procedures.Select(x => new NameAndSchema(x.Schema, x.Table)).ToList()).ToList();

                    var procedureGenerationSettings = GetGenerationSettings();

                    foreach (var procedure in procedureDefinitions)
                    {
                        if (procedureGenerationSettings.ContainsKey(procedure.Name))
                        {
                            var setting = procedureGenerationSettings[procedure.Name];
                            for (int i = 0; i < procedure.ResultSets.Count; i++)
                            {
                                if (setting.Count > i - 1 && setting[i] != null)
                                {
                                    procedure.ResultSets[i].Name = setting[i];
                                }

                            }
                        }
                    }

                    DoWork(() =>
                    {
                        procedureDefinitions.ForEach(x =>
                        {
                            LogMessage($"Processing Table {x.Schema}.{x.Name}");

                            var procedure = _generator.BuildProcedure(x);
                        // var model = _generator.ModelForTable(new RepositoryGenerationObject(x, tableDefinitions)).ToString();
                        //
                        CreateProcedure(x, _generator, procedure);
                        });
                    }, () => Process.Start("explorer.exe", _generationSettings.OutputDirectory));
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

                    if (!Directory.Exists($"{App.ClientDataPath}ProcedureSelections"))
                        Directory.CreateDirectory($"{App.ClientDataPath}ProcedureSelections");

                    var presets = new DirectoryInfo($"{App.ClientDataPath}ProcedureSelections").GetFiles();

                    if (presets.All(x => x.Name.Split('.')[0] != f))
                    {
                        var tableSelection = new TableSelection
                        {
                            Name = f,
                            SelectedTables = Procedures.Where(x => x.Selected).Select(x => $"{x.Schema}.{x.Table}").ToList()
                        };

                        var json = JsonConvert.SerializeObject(tableSelection);

                        File.WriteAllText($@"{App.ClientDataPath}ProcedureSelections\{f}.json", json);
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

                    if (!Directory.Exists($"{App.ClientDataPath}ProcedureSelections"))
                        Directory.CreateDirectory($"{App.ClientDataPath}ProcedureSelections");

                    var presets = new DirectoryInfo($"{App.ClientDataPath}ProcedureSelections").GetFiles();

                    var template = presets.FirstOrDefault(x => x.Name.Split('.')[0] == f);
                    if (template == null) return;

                    var obj = JsonConvert.DeserializeObject<TableSelection>(File.ReadAllText(template.FullName));

                    foreach (var objSelectedTable in obj.SelectedTables)
                    {
                        var table = Procedures.FirstOrDefault(x => $"{x.Schema}.{x.Table}" == objSelectedTable);
                        if (table != null)
                            table.Selected = true;
                    }
                });
            }
        }

        public CreateProceduresViewModel()
        {
            _generationSettings = IOC.Resolve<IOptions<GenerationOptions>>().Value;
            _systemSettings = IOC.Resolve<IOptions<SystemOptions>>().Value;
            _dataSource = IOC.Resolve<DataSourceResolver>().Invoke(_systemSettings.DataSource);
            _generator = IOC.Resolve<GeneratorResolver>().Invoke(_systemSettings.DataSource, _systemSettings.GenerationLanguage);
        }

        internal void CreateProcedure(ProcedureGenerationObject procedureGenerationObject, IGenerator generator, string content)
        {
            var outputDirectory = $"{_generationSettings.OutputDirectory}/Procedures";

            var result = procedureGenerationObject.Name;

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

            if (!Directory.Exists(_generationSettings.OutputDirectory))
                Directory.CreateDirectory(_generationSettings.OutputDirectory);
            if (!Directory.Exists(outputDirectory))
                Directory.CreateDirectory(outputDirectory);

            LogMessage($"Creating Procedure File for {procedureGenerationObject.Schema}.{procedureGenerationObject.Name} in {outputDirectory}/");
            File.WriteAllText(fileName, content);
            LogMessage($"Done {procedureGenerationObject.Schema}.{procedureGenerationObject.Name}!");
        }

        internal void CreateBaseModel(IGenerator generator)
        {
            var outputDirectory = $"{_generationSettings.OutputDirectory}/Models";

            if (!Directory.Exists($"{outputDirectory}/Base"))
                Directory.CreateDirectory($"{outputDirectory}/Base");

            //Write base model
            var baseModel = _generator.BuildBaseModel();
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