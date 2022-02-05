using System.Windows.Controls;
using System.Windows.Input;
using RepoLite.GeneratorEngine.Models;
using RepoLite.ViewModel.Generation;

namespace RepoLite.Views.Generation
{
    public partial class CreateProceduresView : UserControl
    {
        public CreateProceduresView()
        {
            DataContext = IOC.Resolve<CreateProceduresViewModel>();
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListViewItem)?.DataContext is EntityToGenerate item)
                item.Selected = !item.Selected;
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}