using RepoLite.Common.Enums;
using RepoLite.Common.Extensions;
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

        public string LowerClassName => ClassName.ToLower();

        public string ClassName => DbTableName.ToModelName();

        public string RepositoryName => DbTableName.ToRepositoryName();

        public Column GetColumn(string columnName)
        {
            return Columns.Single(x => string.Compare(x.PropertyName, columnName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public Column this[string columnName] => GetColumn(columnName);

        public PrimaryKeyConfigurationEnum PrimaryKeyConfiguration
        {
            get
            {
                var keyCount = Columns.Count(x => x.PrimaryKey);

                switch (keyCount)
                {
                    case 0:
                        return PrimaryKeyConfigurationEnum.NoKey;
                    case 1:
                        return PrimaryKeyConfigurationEnum.PrimaryKey;
                    default:
                        return PrimaryKeyConfigurationEnum.CompositeKey;
                }
            }
        }

        public List<Column> PrimaryKeys
        {
            get { return Columns.Where(x => x.PrimaryKey).ToList(); }
        }

        public List<Column> ForeignKeys
        {
            get { return Columns.Where(x => x.ForeignKey).ToList(); }
        }

        public List<Column> NonPrimaryKeys
        {
            get { return Columns.Where(x => !x.PrimaryKey).ToList(); }
        }

        //Returns a list of table that inherit this table
        public List<Table> GetInheritingTables(List<Table> otherTables)
        {
            var inheritingTables = new List<Table>();
            foreach (var otherTable in otherTables)
            {
                var inheritedDependency =
                    otherTable.ForeignKeys.FirstOrDefault(x => otherTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                if (inheritedDependency != null && inheritedDependency.ForeignKeyTargetTable == DbTableName)
                    inheritingTables.Add(otherTable);
            }

            return inheritingTables;
        }

        public Table Inherits(List<Table> otherTables)
        {
            var inheritedDependency =
                ForeignKeys.FirstOrDefault(x => PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));

            if (inheritedDependency != null)
                return otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            return null;
        }
    }
}
