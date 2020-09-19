using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;
using RepoLite.Generator.DotNet.Generators.Base;

namespace RepoLite.Generator.DotNet.Generators
{
    internal sealed class NoKeyWithInheritance : BaseInheritedGenerator
    {
        private Table _inheritedTable;

        public NoKeyWithInheritance(
            IOptions<GenerationSettings> generationSettings,
            Table table,
            Table inheritedTable) : base(generationSettings, table, inheritedTable)
        {
        }
    }
}
