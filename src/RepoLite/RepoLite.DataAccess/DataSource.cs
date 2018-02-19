using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.DataAccess.Accessors;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace RepoLite.DataAccess
{
    public abstract class DataSource : IDataSource
    {
        public static IDataSource GetDataSource()
        {
            IDataSource dataAccess;
            switch (AppSettings.System.DataSource)
            {
                case DataSourceEnum.SQLServer:
                    dataAccess = new SQLServerAccess();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return dataAccess;
        }

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

    public abstract class DataSource<T> : DataSource where T : DbConnection, new() //dbconnection may not be appropriate going forward, but will do for now
    {
        protected DbConnection Connection => new T { ConnectionString = AppSettings.System.ConnectionString };
    }
}
