using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.DataAccess.Accessors;
using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.Extensions.Options;
using RepoLite.Common.Options;

namespace RepoLite.DataAccess
{
    public abstract class DataSource : IDataSource
    {
        private GenerationOptions _generationSettings;
        public DataSource(
            IOptions<GenerationOptions> generationOptions)
        {
            _generationSettings = generationOptions.Value;
        }
        public abstract List<TableAndSchema> GetTables();
        public abstract List<TableAndSchema> GetTables(string schema);
        public List<Table> LoadTables(List<TableAndSchema> tables)
        {
            var createdTables = new List<Table>();
            foreach (var table in tables)
            {
                var item = new Table(_generationSettings)
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
