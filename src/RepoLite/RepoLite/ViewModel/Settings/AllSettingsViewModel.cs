using RepoLite.Commands;
using RepoLite.Common.Enums;
using RepoLite.Common.Settings;
using RepoLite.ViewModel.Base;
using RepoLite.Views.Settings;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RepoLite.ViewModel.Settings
{
    public class AllSettingsViewModel : ViewModelBase
    {
        private readonly IWritableOptions<GenerationSettings> _generationSettings;
        private readonly IWritableOptions<SystemSettings> _systemSettings;

        public GenerationSettings GenerationSettings => _generationSettings.Value;
        public SystemSettings SystemSettings => _systemSettings.Value;

        public AllSettingsViewModel(
            IWritableOptions<GenerationSettings> generationSettings,
            IWritableOptions<SystemSettings> systemSettings)
        {
            _generationSettings = generationSettings;
            _systemSettings = systemSettings;
        }

        private bool _connectionTestEnabled = true;
        public bool ConnectionTestEnabled
        {

            get => _connectionTestEnabled;
            set => SetProperty(ref _connectionTestEnabled, value);
        }

        private string _connectionTestResult;
        public string ConnectionTestResult
        {

            get => _connectionTestResult;
            set => SetProperty(ref _connectionTestResult, value);
        }

        public ICommand TestDatabaseConnection
        {
            get
            {
                return new RelayCommand(o =>
                {
                    //Save settings prior to test

                    ConnectionTestResult = "Working";
                    ConnectionTestEnabled = false;

                    switch (SystemSettings.DataSource)
                    {
                        case DataSourceEnum.SQLServer:
                            DoWork(async () => ConnectionTestResult = await TestSQLServerConnection());
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                });
            }
        }

        private async Task<string> TestSQLServerConnection()
        {
            try
            {
                //This checks the format of the connectionstring
                // ReSharper disable once ObjectCreationAsStatement
                new DbConnectionStringBuilder
                {
                    ConnectionString = SystemSettings.ConnectionString
                };
            }
            catch
            {
                ConnectionTestEnabled = true;
                return await Task.FromResult("Invalid format.");
            }

            try
            {
                new SqlConnection(SystemSettings.ConnectionString).Open();
            }
            catch
            {
                ConnectionTestEnabled = true;
                return await Task.FromResult("Unable to connect.");
            }

            ConnectionTestEnabled = true;
            return await Task.FromResult("Valid");
        }


        private string _connectionString;

        public string ConnectionString
        {
            get => _connectionString;
            set => SetProperty(ref _connectionString, value);
        }
        public ICommand SaveAndClose
        {
            get
            {
                return new RelayCommand(o =>
                {
                    _generationSettings.Update(opt =>
                    {
                        opt.ModelGenerationNamespace = GenerationSettings.ModelGenerationNamespace;
                        opt.RepositoryGenerationNamespace = GenerationSettings.RepositoryGenerationNamespace;
                        opt.OutputDirectory = GenerationSettings.OutputDirectory;
                        opt.ModelFileNameFormat = GenerationSettings.ModelFileNameFormat;
                        opt.RepositoryFileNameFormat = GenerationSettings.RepositoryFileNameFormat;
                        opt.ModelClassNameFormat = GenerationSettings.ModelClassNameFormat;
                        opt.RepositoryClassNameFormat = GenerationSettings.RepositoryClassNameFormat;
                        opt.GenerateSealedObjects = GenerationSettings.GenerateSealedObjects;

                    });

                    _systemSettings.Update(opt =>
                    {
                        opt.ConnectionString = SystemSettings.ConnectionString;
                    });

                    var wnd = o as Global;
                    NavigationCommands.BrowseBack.Execute(null, wnd);
                });
            }
        }
    }
}
