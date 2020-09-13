using Microsoft.Extensions.DependencyInjection;
using RepoLite.ViewModel.Main;
using RepoLite.ViewModel.Settings;

namespace RepoLite.ViewModel
{
    public class ViewModelLocator
    {
        public LandingViewModel LandingViewModel => App.ServiceProvider.GetRequiredService<LandingViewModel>();

        public CreateModelsViewModel CreateModelsViewModel => App.ServiceProvider.GetRequiredService<CreateModelsViewModel>();

        public CreateRepositoriesViewModel CreateRepositoriesViewModel => App.ServiceProvider.GetRequiredService<CreateRepositoriesViewModel>();

        public AllSettingsViewModel AllSettingsViewModel => App.ServiceProvider.GetRequiredService<AllSettingsViewModel>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}