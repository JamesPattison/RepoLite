using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using System.Collections.Generic;

namespace RepoLite.DataAccess
{
    public abstract class DataSource : IDataSource
    {
        public abstract List<TableAndSchema> GetTables();
        public abstract List<TableAndSchema> GetTables(string schema);
        public List<Table> LoadTables(List<TableAndSchema> tables)
        {
            var createdTables = new List<Table>();
            foreach (var table in tables)
            {
                var item = new Table
                {
                    DbTableName = table.Table,
                    Schema = table.Schema
                };

                item.Columns = LoadTableColumns(item);

                createdTables.Add(item);
            }

            return createdTables;
        }
        public abstract List<string> GetProcedures();
        public abstract List<Procedure> LoadProcedures(List<string> procedures);
        //protected abstract List<TableDefault> GetTableDefaults(List<TableAndSchema> tables);
        public abstract List<Column> LoadTableColumns(Table table);
    }
}
