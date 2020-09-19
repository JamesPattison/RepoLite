using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;
using RepoLite.Generator.DotNet.Generators.Base;

namespace RepoLite.Generator.DotNet.Generators
{
    internal sealed class PrimaryKey : BaseGenerator
    {
        public PrimaryKey(
            IOptions<GenerationSettings> generationSettings,
            Table table) : base(generationSettings, table)
        {
        }
    }
}
