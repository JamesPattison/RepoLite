using RepoLite.Common;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using System.IO;
using Microsoft.Extensions.Options;
using RepoLite.Common.Options;

namespace RepoLite.GeneratorEngine.Generators.BaseParsers
{
    public class CSharpSqlServerBaseClassParser : IParser
    {
        private GenerationOptions _generationSettings;

        public CSharpSqlServerBaseClassParser(IOptions<GenerationOptions> generationOptions)
        {
            _generationSettings = generationOptions.Value;
        }
        public string BuildBaseRepository()
        {
            var template = File.ReadAllText(@"Templates\CSharp\SqlServer\BaseRepository.cs.txt");
            template = template
                .Replace("REPOSITORYNAMESPACE", _generationSettings.RepositoryGenerationNamespace)
                .Replace("MODELNAMESPACE", _generationSettings.ModelGenerationNamespace);
            return template;
        }

        public string BuildBaseModel()
        {
            var template = File.ReadAllText(@"Templates\CSharp\SqlServer\BaseModel.cs.txt");
            template = template
                .Replace("REPOSITORYNAMESPACE", _generationSettings.RepositoryGenerationNamespace)
                .Replace("MODELNAMESPACE", _generationSettings.ModelGenerationNamespace);
            return template;
        }
    }
}
