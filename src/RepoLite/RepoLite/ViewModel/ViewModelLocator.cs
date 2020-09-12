using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using RepoLite.ViewModel.Main;
using RepoLite.ViewModel.Settings;

namespace RepoLite.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            //Main
            SimpleIoc.Default.Register<LandingViewModel>();
            SimpleIoc.Default.Register<CreateModelsViewModel>();
            SimpleIoc.Default.Register<CreateRepositoriesViewModel>();

            //Settings
            SimpleIoc.Default.Register<AllSettingsViewModel>();

        }

        #region Main

        public LandingViewModel LandingViewModel => ServiceLocator.Current.GetInstance<LandingViewModel>();

        public CreateModelsViewModel CreateModelsViewModel => ServiceLocator.Current.GetInstance<CreateModelsViewModel>();

        public CreateRepositoriesViewModel CreateRepositoriesViewModel => ServiceLocator.Current.GetInstance<CreateRepositoriesViewModel>();

        #endregion

        #region Settings

        public AllSettingsViewModel AllSettingsViewModel => ServiceLocator.Current.GetInstance<AllSettingsViewModel>();

        #endregion

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}