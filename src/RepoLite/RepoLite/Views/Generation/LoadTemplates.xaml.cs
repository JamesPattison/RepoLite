using RepoLite.Commands;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace RepoLite.Views.Generation
{
    /// <summary>
    /// Interaction logic for LoadTemplates.xaml
    /// </summary>
    public sealed partial class LoadTemplates : Window
    {
        public LoadTemplates()
        {
            DataContext = this;
            InitializeComponent();
            if (!Directory.Exists($"{App.ClientDataPath}TableSelections"))
                Directory.CreateDirectory($"{App.ClientDataPath}TableSelections");

            var presets = new DirectoryInfo($"{App.ClientDataPath}TableSelections").GetFiles().Select(x => x.Name.Split('.')[0]);
            foreach (var preset in presets)
            {
                Items.Add(preset);
            }
        }

        public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();

        public string SelectedItem { get; set; }

        public ICommand Load
        {
            get { return new RelayCommand(o => { base.Close(); }); }
        }

        public new ICommand Close
        {
            get
            {
                return new RelayCommand(o =>
                {
                    SelectedItem = String.Empty;
                    base.Close();
                });
            }
        }
    }
}
