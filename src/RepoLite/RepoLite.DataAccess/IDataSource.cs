using RepoLite.Common.Models;
using System.Collections.Generic;
using RepoLite.Common.Enums;

namespace RepoLite.DataAccess
{
    public delegate IDataSource DataSourceResolver(DataSourceEnum dataSource);
    public interface IDataSource
    {
        IEnumerable<NameAndSchema> GetTables();
        IEnumerable<NameAndSchema> GetTables(string schema);
        IEnumerable<ProcedureDefinition> GetProcedures();
        
        
        IEnumerable<Table> LoadTables(IEnumerable<NameAndSchema> tables);
        IEnumerable<ProcedureGenerationObject> LoadProcedures(IEnumerable<ProcedureDefinition> procedures);
        
        IEnumerable<Column> LoadTableColumns(Table table);
    }
}
