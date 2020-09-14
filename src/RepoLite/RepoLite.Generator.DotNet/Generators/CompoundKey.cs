using Microsoft.Extensions.Options;
using RepoLite.Common.Settings;
using RepoLite.Generator.DotNet.Generators.Base;

namespace RepoLite.Generator.DotNet.Generators
{
    internal sealed class CompoundKey : BaseGenerator
    {
        public CompoundKey(IOptions<GenerationSettings> generationSettings) : base(generationSettings)
        {
        }
    }
}
