using RepoLite.Commands;
using RepoLite.ViewModel.Base;
using System.Windows.Input;
using RepoLite.GeneratorEngine.Generators;
using RepoLite.GeneratorEngine.Generators.CSharp.SQLServer;
using RepoLite.Views;

namespace RepoLite.ViewModel
{
    public class LandingViewModel : ViewModelBase
    {

        public LandingViewModel()
        {
            var text = new Repository(new RepositoryGenerationObject {Test = "Dicks"}).TransformText();
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
