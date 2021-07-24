using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoLite.Common.Models
{
    public class RepositoryGenerationObject
    {
        /// <summary>
        /// The table this object refers to
        /// </summary>
        public Table Table { get; set; }
        
        /// <summary>
        /// If this table inherits another in the database, this is the object used
        /// </summary>
        public Table InheritedTable { get; set; }
        
        public RepositoryGenerationObject(Table table, IEnumerable<Table> otherTables)
        {
            table.Columns = table.Columns.Where(x => string.IsNullOrEmpty(x.DbTableName) || x.DbTableName == table.DbTableName).ToList();
            foreach (var inheritedTableColumn in table.Columns)
            {
                inheritedTableColumn.DbTableName = table.DbTableName;
            }
            
            BuildInheritedColumns(ref table, table, otherTables);
            
            //todo clean up below
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y =>
                    y.DbColumnName == x.DbColumnName && y.ForeignKeyTargetTable != table.DbTableName));

            if (inheritedDependency == null)
            {
                Table = table;
                return;
            }
            
            var inheritedTable =
                otherTables.FirstOrDefault(x =>
                    x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            if (inheritedTable == null) return;
            
            InheritedTable = inheritedTable;

            Table = RemoveInheritedPrimaryKeys(table);
        }

        private Table RemoveInheritedPrimaryKeys(Table table)
        {
            table.Columns = table.Columns.Where(x => x.DbTableName == table.DbTableName || !x.PrimaryKey).ToList();
            return table;
        }

        // public TableToGenerate InheritedTable2 { get; set; }
        // public IEnumerable<TableColumn> Columns { get; set; }

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
    }
}