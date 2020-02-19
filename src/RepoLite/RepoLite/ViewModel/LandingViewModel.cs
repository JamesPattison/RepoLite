using RepoLite.Commands;
using RepoLite.ViewModel.Base;
using System.Windows.Input;
using RepoLite.Views;

namespace RepoLite.ViewModel
{
    public class LandingViewModel : ViewModelBase
    {
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
