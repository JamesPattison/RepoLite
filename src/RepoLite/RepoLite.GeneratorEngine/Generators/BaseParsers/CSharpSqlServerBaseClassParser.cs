using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using System.Collections.Generic;
using System.IO;

namespace RepoLite.GeneratorEngine.Generators.BaseParsers
{
    public class CSharpSqlServerBaseClassParser : BaseClassParser
    {
        public CSharpSqlServerBaseClassParser(TargetFramework targetFramework, CSharpVersion cSharpVersion) :
            base(new List<string> { targetFramework.ToString(), cSharpVersion.ToString() })
        {
        }

        public CSharpSqlServerBaseClassParser(TargetFramework targetFramework, CSharpVersion cSharpVersion,
            BaseClassParseOptions parseOptions) : base(
            new List<string> { targetFramework.ToString(), cSharpVersion.ToString() }, parseOptions)
        {
        }

        public override string BuildBaseRepository()
        {
            var template = File.ReadAllText(@"Templates\CSharp\BaseRepository.txt");
            template = template
                .Replace("{{NAMESPACE}}", AppSettings.Generation.RepositoryGenerationNamespace)
                .Replace("{{MODELNAMESPACE}}", AppSettings.Generation.ModelGenerationNamespace);
            return Parse(template);
        }

        public override string BuildBaseModel()
        {
            var template = File.ReadAllText(@"Templates\CSharp\BaseModel.txt");
            template = template.Replace("{{NAMESPACE}}", AppSettings.Generation.ModelGenerationNamespace);
            return Parse(template);
        }
    }
}
