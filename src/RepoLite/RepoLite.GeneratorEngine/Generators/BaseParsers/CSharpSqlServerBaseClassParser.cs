using Microsoft.Extensions.Options;
using RepoLite.Common.Settings;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using System.IO;

namespace RepoLite.GeneratorEngine.Generators.BaseParsers
{
    public class CSharpSqlServerBaseClassParser : IParser
    {
        private readonly IOptions<GenerationSettings> _generationSettings;

        public CSharpSqlServerBaseClassParser(IOptions<GenerationSettings> generationSettings)
        {
            _generationSettings = generationSettings;
        }

        public string BuildBaseRepository()
        {
            var template = File.ReadAllText(@"Templates\CSharp\SqlServer\BaseRepository.cs.txt");
            template = template
                .Replace("REPOSITORYNAMESPACE", _generationSettings.Value.RepositoryGenerationNamespace)
                .Replace("MODELNAMESPACE", _generationSettings.Value.ModelGenerationNamespace);
            return template;
        }

        public string BuildBaseModel()
        {
            var template = File.ReadAllText(@"Templates\CSharp\SqlServer\BaseModel.cs.txt");
            template = template
                .Replace("REPOSITORYNAMESPACE", _generationSettings.Value.RepositoryGenerationNamespace)
                .Replace("MODELNAMESPACE", _generationSettings.Value.ModelGenerationNamespace);
            return template;
        }
    }
}
