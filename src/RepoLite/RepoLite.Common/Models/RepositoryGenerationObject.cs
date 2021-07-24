using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoLite.Common.Models
{
    public class RepositoryGenerationObject
    {
        public RepositoryGenerationObject(Table table, IEnumerable<Table> otherTables)
        {
            Table = table;
            foreach (var inheritedTableColumn in Table.Columns)
            {
                inheritedTableColumn.DbTableName = Table.DbTableName;
            }
            
            BuildInheritedColumns(ref table, table, otherTables);
        }

        private void BuildInheritedColumns(ref Table topLevelTable, Table table, IEnumerable<Table> otherTables)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y =>
                    y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));

            if (inheritedDependency == null) return;
            
            var inheritedTable =
                otherTables.FirstOrDefault(x =>
                    x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable == null) return;

            foreach (var inheritedTableColumn in inheritedTable.Columns)
            {
                inheritedTableColumn.DbTableName = inheritedTable.DbTableName;
                topLevelTable.Columns.Add(inheritedTableColumn);
            }

            BuildInheritedColumns(ref topLevelTable, inheritedTable, otherTables);
        }

        /// <summary>
        /// The table this object refers to
        /// </summary>
        public Table Table { get; set; }
        
        /// <summary>
        /// If this table inherits another in the database, this is the object used
        /// </summary>
        [Obsolete]
        public RepositoryGenerationObject InheritedTable { get; set; }
        
        /// <summary>
        /// The Column which is the dependency between this and the InheritedTable
        /// </summary>
        [Obsolete]
        public Column InheritedDependency { get; set; }
    }
}