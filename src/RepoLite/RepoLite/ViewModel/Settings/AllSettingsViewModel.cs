using RepoLite.Commands;
using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.ViewModel.Base;
using RepoLite.Views.Settings;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.Options;
using RepoLite.Common.Options;

namespace RepoLite.ViewModel.Settings
{
    public class AllSettingsViewModel : ViewModelBase
    {
        public SystemOptions SystemSettings { get; set; }
        public GenerationOptions GenerationSettings { get; set; }

        public AllSettingsViewModel()
        {
            SystemSettings = IOC.Resolve<IOptionsSnapshot<SystemOptions>>().Value;
            GenerationSettings = IOC.Resolve<IOptionsSnapshot<GenerationOptions>>().Value;
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
                    GenerationSettings.Save();
                    SystemSettings.Save();
                    var wnd = o as Global;
                    NavigationCommands.BrowseBack.Execute(null, wnd);
                });
            }
        }
    }
}
