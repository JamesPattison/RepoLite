using RepoLite.Commands;
using RepoLite.ViewModel.Base;
using System.Windows.Input;
using RepoLite.Views;

namespace RepoLite.ViewModel
{
    public class LandingViewModel : ViewModelBase
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        public LandingViewModel()
        {
            Text = @"
using RepoLite.Common.Models;
using System.Collections.Generic;

namespace RepoLite.DataAccess
{
    public interface IDataSource
    {
        List<TableAndSchema> GetTables();
        List<TableAndSchema> GetTables(string schema);
        List<Table> LoadTables(List<TableAndSchema> tables);

        List<string> GetProcedures();
        List<Procedure> LoadProcedures(List<string> procedures);

        List<string> GetTableColumns(Table table);
        List<Column> LoadTableColumns(Table table);
    }
}
";
        }

        public ICommand NavigateToCreateModels
        {
            get
            {
                return new RelayCommand(o =>
                {
                    DoWork(() =>
                    {
                        var wnd = o as LandingView;
                        Execute("/Views/Generation/CreateModelsView.xaml", wnd);
                    });
                });
            }
        }

        public ICommand NavigateToCreateRepositories
        {
            get
            {
                return new RelayCommand(o =>
                {
                    DoWork(() =>
                    {
                        var wnd = o as LandingView;
                        Execute("/Views/Generation/CreateRepositoriesView.xaml", wnd);
                    });
                });
            }
        }
    }
}
