using System.Linq;
using RepoLite.Commands;
using RepoLite.ViewModel.Base;
using RepoLite.Views;
using System.Windows.Input;
using Microsoft.Extensions.Options;
using RepoLite.Common.Options;
using RepoLite.DataAccess;

namespace RepoLite.ViewModel
{
    public class LandingViewModel : ViewModelBase
    {
        public LandingViewModel()
        {
            var systemSettings = IOC.Resolve<IOptions<SystemOptions>>().Value;
            var dataSource = IOC.Resolve<DataSourceResolver>().Invoke(systemSettings.DataSource);
            
            var procedures = dataSource.GetProcedures();

            var loadedProcedures = dataSource.LoadProcedures(procedures);
            var g = 12;
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

        public ICommand NavigateToCreateProcedures
        {
            get
            {
                return new RelayCommand(o =>
                {
                    DoWork(() =>
                    {
                        var wnd = o as LandingView;
                        Execute("/Views/Generation/CreateProceduresView.xaml", wnd);
                    });
                });
            }
        }
    }
}
