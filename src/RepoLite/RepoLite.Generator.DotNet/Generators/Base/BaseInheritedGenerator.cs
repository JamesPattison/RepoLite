using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        protected void DoShitRecursively(StringBuilder sb, Column dependency, Action<Column, Table> shitToDo)
        {
            var inheritedTable =
                _otherTables.FirstOrDefault(x => x.DbTableName == dependency.ForeignKeyTargetTable);
            if (inheritedTable != null)
            {
                var inheritedTableInheritedDependency =
                    inheritedTable.ForeignKeys.FirstOrDefault(x => inheritedTable.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));
                var inheritedTableAlsoInherits = inheritedTableInheritedDependency != null;

                shitToDo(dependency, inheritedTable);

                if (inheritedTableAlsoInherits)
                {
                    DoShitRecursively(sb, inheritedTableInheritedDependency, shitToDo);
                }
            }
        }
    }
}