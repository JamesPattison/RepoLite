
using RepoLite.ViewModel.Settings;

namespace RepoLite.Views.Settings
{
    /// <summary>
    /// Interaction logic for Global.xaml
    /// </summary>
    public partial class Global
    {
        public Global()
        {
            DataContext = IOC.Resolve<AllSettingsViewModel>();
            InitializeComponent();
        }
    }
}
