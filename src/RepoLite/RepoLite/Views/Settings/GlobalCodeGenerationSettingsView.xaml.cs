using RepoLite.ViewModel.Settings;

namespace RepoLite.Views.Settings
{
    /// <summary>
    /// Interaction logic for CodeGeneration.xaml
    /// </summary>
    public partial class GlobalCodeGenerationSettingsView
    {
        public GlobalCodeGenerationSettingsView()
        {
            DataContext = IOC.Resolve<AllSettingsViewModel>();
            InitializeComponent();
        }
    }
}
