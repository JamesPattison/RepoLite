using Microsoft.Extensions.Options;
using RepoLite.Common.Settings;
using RepoLite.Generator.DotNet.Generators.Base;

namespace RepoLite.Generator.DotNet.Generators
{
    internal sealed class NoKey : BaseGenerator
    {
        public NoKey(IOptions<GenerationSettings> generationSettings) : base(generationSettings)
        {
        }
    }
}
