using Microsoft.Extensions.Options;
using RepoLite.Common.Extensions;
using RepoLite.Common.Settings;

namespace RepoLite.Generator.DotNet.Generators.Base
{
    internal class BaseGenerator
    {
        private readonly GenerationSettings _generationSettings;

        public BaseGenerator(
            IOptions<GenerationSettings> generationSettings)
        {
            _generationSettings = generationSettings.Value;
        }

        protected internal string FileExtension()
        {
            return "cs";
        }

        protected internal string RepositoryName(string repository)
        {
            return repository.ToRepositoryName(_generationSettings.RepositoryClassNameFormat);
        }

        protected internal string ModelName(string model)
        {
            return model.ToModelName(_generationSettings.ModelClassNameFormat);
        }
    }
}
