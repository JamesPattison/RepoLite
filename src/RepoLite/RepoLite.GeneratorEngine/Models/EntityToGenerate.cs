using RepoLite.Common.Models;

namespace RepoLite.GeneratorEngine.Models
{
    public class EntityToGenerate : NotifyChangingObject
    {
        private bool _selected;
        private string _schema;
        private string _table;
        private string _definition;
        
        public bool Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string Schema
        {
            get => _schema;
            set => SetProperty(ref _schema, value);
        }

        public string Table
        {
            get => _table;
            set => SetProperty(ref _table, value);
        }

        public string Definition
        {
            get => _definition;
            set => SetProperty(ref _definition, value);
        }
    }
}
