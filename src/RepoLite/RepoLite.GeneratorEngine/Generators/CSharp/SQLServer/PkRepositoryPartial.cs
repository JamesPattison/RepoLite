using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Options;

namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer
{
    public partial class PkRepository
    {
        private readonly GenerationOptions generationSettings;
        private readonly RepositoryGenerationObject generationObject;

        public PkRepository(
            IOptions<GenerationOptions> genOptions,
            RepositoryGenerationObject generationObject)
        {
            generationSettings = genOptions.Value;
            this.generationObject = generationObject;
        }
    }
}