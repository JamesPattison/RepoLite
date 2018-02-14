using FirstFloor.ModernUI.Presentation;
using System.Linq;
using System.Windows.Media;

namespace RepoLite.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            AppearanceManager.Current.AccentColor = Colors.DarkViolet;
            ContentSource = MenuLinkGroups.First().Links.First().Source;
        }
    }
}
