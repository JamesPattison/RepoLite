using RepoLite.Common.Models;
using System.Collections.Generic;

namespace RepoLite.Common.Interfaces
{
    public delegate IDataSource DataSourceResolver();
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
