using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using RepoLite.Common.Enums;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;

namespace RepoLite.Generator.DotNet.Generators.Base
{
    public class GeneratorFactory
    {
        private readonly IOptions<GenerationSettings> _generationSettings;

        public GeneratorFactory(IOptions<GenerationSettings> generationSettings)
        {
            _generationSettings = generationSettings;
        }
        
        public IGenerator Get(Table table, List<Table> otherTables)
        {
            var inheritedDependency =
                table.ForeignKeys.FirstOrDefault(x => table.PrimaryKeys.Any(y => y.DbColumnName == x.DbColumnName));

            Table inheritedTable = null;
            if (inheritedDependency != null)
                inheritedTable = otherTables.FirstOrDefault(x => x.DbTableName == inheritedDependency.ForeignKeyTargetTable);

            return table.PrimaryKeyConfiguration switch
            {
                PrimaryKeyConfigurationEnum.NoKey when inheritedTable == null => new NoKey(_generationSettings, table),
                PrimaryKeyConfigurationEnum.NoKey => new NoKeyWithInheritance(_generationSettings, table, inheritedTable),
                
                PrimaryKeyConfigurationEnum.PrimaryKey when inheritedTable == null => new PrimaryKey(_generationSettings, table),
                PrimaryKeyConfigurationEnum.PrimaryKey => new PrimaryKeyWithInheritance(_generationSettings, table, inheritedTable),
                
                PrimaryKeyConfigurationEnum.CompositeKey when inheritedTable == null => new CompoundKey(_generationSettings, table),
                PrimaryKeyConfigurationEnum.CompositeKey => new CompoundKeyWithInheritance(_generationSettings, table, inheritedTable),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}