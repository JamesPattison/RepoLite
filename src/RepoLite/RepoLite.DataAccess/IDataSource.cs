using RepoLite.Common.Models;
using System.Collections.Generic;
using RepoLite.Common.Enums;

namespace RepoLite.DataAccess
{
    public delegate IDataSource DataSourceResolver(DataSourceEnum dataSource);
    public interface IDataSource
    {
        List<TableAndSchema> GetTables();
        List<TableAndSchema> GetTables(string schema);
        List<Table> LoadTables(List<TableAndSchema> tables);

        List<string> GetProcedures();
        List<Procedure> LoadProcedures(List<string> procedures);
        
        List<Column> LoadTableColumns(Table table);
    }
}
