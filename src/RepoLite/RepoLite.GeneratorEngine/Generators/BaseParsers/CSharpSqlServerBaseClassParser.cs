using RepoLite.Common;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using System.IO;

namespace RepoLite.GeneratorEngine.Generators.BaseParsers
{
    public class CSharpSqlServerBaseClassParser : IParser
    {
        public string BuildBaseRepository()
        {
            var template = File.ReadAllText(@"Templates\CSharp\BaseRepository.cs");
            template = template
                .Replace("REPOSITORYNAMESPACE", AppSettings.Generation.RepositoryGenerationNamespace)
                .Replace("MODELNAMESPACE", AppSettings.Generation.ModelGenerationNamespace);
            return template;
        }

        public string BuildBaseModel()
        {
            var template = File.ReadAllText(@"Templates\CSharp\BaseModel.cs");
            template = template.Replace("MODELNAMESPACE", AppSettings.Generation.ModelGenerationNamespace);
            return template;
        }
    }
}
