using RepoLite.Common.Models;

namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer
{
    public partial class Repository
    {
        private readonly RepositoryGenerationObject generationObject;

        public Repository(RepositoryGenerationObject generationObject)
        {
            this.generationObject = generationObject;
        }
    }
}