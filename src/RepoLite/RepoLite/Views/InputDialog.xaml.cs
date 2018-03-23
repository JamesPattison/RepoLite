using RepoLite.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace RepoLite.Views
{
    /// <summary>
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window, INotifyPropertyChanged
    {
        private string _value = "";

        public InputDialog()
        {
            DataContext = this;
            InitializeComponent();
            inp.Focus();
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }

        public ICommand Save
        {
            get { return new RelayCommand(o => { base.Close(); }); }
        }

        public new ICommand Close
        {
            get
            {
                return new RelayCommand(o =>
                {
                    base.Close();
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
