using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;

namespace RepoLite.Generator.DotNet.Generators.Base
{
    internal abstract class BaseInheritedGenerator : BaseGenerator
    {
        protected readonly IEnumerable<Table> _otherTables;
        protected readonly Column _inheritedDependency;
        protected readonly Table _inheritedTable;

        protected BaseInheritedGenerator(IOptions<GenerationSettings> generationSettings,
            Table table,
            IEnumerable<Table> otherTables) : base(generationSettings, table)
        {
            _otherTables = otherTables;
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));

            Table inheritedTable = null;
            if (inheritedDependency != null)
                inheritedTable = otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);
            
            _inheritedDependency = inheritedDependency;
            _inheritedTable = inheritedTable;
        }
    }
}