using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RepoLite.Common.Models
{
    public class NotifyChangingObject : INotifyPropertyChanged
    {
        protected bool SetProperty<T>(
            ref T storage,
            T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;
            if (propertyName != null)
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
