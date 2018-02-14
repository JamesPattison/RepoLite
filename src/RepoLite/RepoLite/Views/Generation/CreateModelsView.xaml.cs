using RepoLite.GeneratorEngine.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace RepoLite.Views.Main
{
    /// <summary>
    /// Interaction logic for CreateModelsView.xaml
    /// </summary>
    public partial class CreateModelsView
    {
        public CreateModelsView()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            if ((sender as ListViewItem)?.DataContext is TableToGenerate item)
                item.Selected = !item.Selected;
        }
    }
}
