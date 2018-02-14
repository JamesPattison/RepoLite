using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoLite.Common.Models
{
    public class Table
    {
        public List<Column> Columns { get; set; } = new List<Column>();

        public string Schema { get; set; }
        //public string SequenceName;
        //public bool Ignore;
        public string DbTableName { get; set; }
        public string ClassName => Helpers.UpperFirst(DbTableName);

        public string LowerClassName
        {
            get
            {
                var name = Helpers.LowerFirst(DbTableName);
                if (Helpers.ReservedWord(name))
                    name = "@" + name;

                return name;
            }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.Single(x => string.Compare(x.PropertyName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public Column this[string columnName] => GetColumn(columnName);

        public bool HasCompositeKey
        {
            get { return Columns.Count(x => x.PrimaryKey) > 1; }
        }

        public List<Column> PrimaryKeys
        {
            get { return Columns.Where(x => x.PrimaryKey).ToList(); }
        }

        public List<Column> NonPrimaryKeys
        {
            get { return Columns.Where(x => !x.PrimaryKey).ToList(); }
        }
    }
}
