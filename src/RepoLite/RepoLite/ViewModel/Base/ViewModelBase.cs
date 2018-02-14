using RepoLite.Common.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace RepoLite.ViewModel.Base
{
    public abstract class ViewModelBase : NotifyChangingObject
    {
        private bool _isLoading;
        private string _errorMessage;
        protected BackgroundWorker Worker;

        public bool IsDirty { get; set; }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        protected void AddToList<T>(T item, ObservableCollection<T> list)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => list.Add(item)));
        }

        protected void DoWork(Action action)
        {
            DoWork(action, () => { });
        }

        protected void DoWork(Action action, Action callBack)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                ErrorMessage = String.Empty;
                action();
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                callBack();
                CommandManager.InvalidateRequerySuggested();
                IsLoading = false;
            };

            worker.RunWorkerAsync();
        }

        protected void Execute(string parameter, IInputElement target)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => NavigationCommands.GoToPage.Execute(parameter, target)));
        }
    }
}
