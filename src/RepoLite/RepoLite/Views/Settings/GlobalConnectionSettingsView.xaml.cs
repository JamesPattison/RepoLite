using RepoLite.ViewModel.Settings;

namespace RepoLite.Views.Settings
{
    /// <summary>
    /// Interaction logic for Connection.xaml
    /// </summary>
    public partial class GlobalConnectionSettingsView
    {
        public GlobalConnectionSettingsView()
        {
            DataContext = IOC.Resolve<AllSettingsViewModel>();
            InitializeComponent();
        }
    }
}
