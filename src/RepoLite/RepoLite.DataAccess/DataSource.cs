using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.DataAccess.Accessors;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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
        public abstract IEnumerable<NameAndSchema> GetTables();
        public abstract IEnumerable<NameAndSchema> GetTables(string schema);
        public abstract IEnumerable<ProcedureDefinition> GetProcedures();
        public IEnumerable<Table> LoadTables(IEnumerable<NameAndSchema> tables)
        {
            var createdTables = new List<Table>();
            foreach (var table in tables)
            {
                var item = new Table(_generationSettings)
                {
                    DbTableName = table.Name,
                    Schema = table.Schema
                };

                item.Columns = LoadTableColumns(item).ToList();

                createdTables.Add(item);
            }

            return createdTables;
        }
        public abstract IEnumerable<ProcedureGenerationObject> LoadProcedures(IEnumerable<ProcedureDefinition> procedures);
        //protected abstract List<TableDefault> GetTableDefaults(List<TableAndSchema> tables);
        public abstract IEnumerable<Column> LoadTableColumns(Table table);
    }
}
