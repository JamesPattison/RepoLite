using RepoLite.GeneratorEngine.Models;
using System.Windows.Controls;
using System.Windows.Input;
using RepoLite.ViewModel.Generation;

namespace RepoLite.Views.Generation
{
    /// <summary>
    /// Interaction logic for CreateRepositories.xaml
    /// </summary>
    public partial class CreateRepositoriesView
    {
        public CreateRepositoriesView()
        {
            DataContext = IOC.Resolve<CreateRepositoriesViewModel>();
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListViewItem)?.DataContext is EntityToGenerate item)
                item.Selected = !item.Selected;
        }
    }
}
