using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepoLite.Models
{
    public class ListboxItem<T> : INotifyPropertyChanged where T : class
    {
        private T _item;
        private bool _isSelected;

        public T Item
        {
            get => _item;
            set
            {
                _item = value;
                OnPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}