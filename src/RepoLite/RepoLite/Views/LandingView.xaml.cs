using RepoLite.ViewModel;

namespace RepoLite.Views
{
    /// <summary>
    /// Interaction logic for Landing.xaml
    /// </summary>
    public partial class LandingView
    {
        public LandingView()
        {
            DataContext = IOC.Resolve<LandingViewModel>();
            InitializeComponent();
        }
    }
}
