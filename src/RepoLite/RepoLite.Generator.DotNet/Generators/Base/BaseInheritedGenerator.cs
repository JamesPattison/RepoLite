using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;

namespace RepoLite.Generator.DotNet.Generators.Base
{
    internal class BaseInheritedGenerator : BaseGenerator
    {
        protected readonly Table _inheritedTable;

        public BaseInheritedGenerator(
            IOptions<GenerationSettings> generationSettings,
            Table table,
            Table inheritedTable) : base(generationSettings, table)
        {
            _inheritedTable = inheritedTable;
        }
    }
}