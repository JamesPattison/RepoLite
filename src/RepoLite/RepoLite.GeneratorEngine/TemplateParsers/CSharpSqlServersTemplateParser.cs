using System.IO;
using Microsoft.Extensions.Options;
using RepoLite.Common.Settings;
using RepoLite.GeneratorEngine.TemplateParsers.Base;

namespace RepoLite.GeneratorEngine.TemplateParsers
{
    public class CSharpSqlServersTemplateParser : ITemplateParser
    {
        private readonly IOptions<GenerationSettings> _generationSettings;

        public CSharpSqlServersTemplateParser(IOptions<GenerationSettings> generationSettings)
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
