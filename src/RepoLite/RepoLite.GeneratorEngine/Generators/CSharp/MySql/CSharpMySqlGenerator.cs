using System.Linq;
using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Options;
using RepoLite.GeneratorEngine.Generators.CSharp.MySql.Templates.Repositories;
using RepoLite.GeneratorEngine.Generators.CSharp.MySql.Templates.Repositories.Pk;

namespace RepoLite.GeneratorEngine.Generators.CSharp.MySql
{
    public class CSharpMySqlGenerator : CSharpCodeGenerator
    {
        private GenerationOptions _generationOptions;

        public CSharpMySqlGenerator(IOptions<GenerationOptions> generationOptions)
        {
            _generationOptions = generationOptions.Value;
        }
        public override string BuildModel(RepositoryGenerationObject generationObject)
        {
            return TemplateProcessor.ProcessTemplate<Model>(_generationOptions, generationObject);
        }

        public override string BuildRepository(RepositoryGenerationObject generationObject)
        {
            if (generationObject.Table.PrimaryKeys.Any())
            {
                if (generationObject.Table.PrimaryKeys.Count == 1)
                {
                    return TemplateProcessor.ProcessTemplate<PkRepository>(_generationOptions, generationObject);
                }
                else
                {
                }
            }

            return string.Empty;
        }

        public override string BuildProcedure(ProcedureGenerationObject procedureGenerationObject)
        {
            throw new System.NotImplementedException();
        }

        public override string BuildBaseRepository()
        {
            return TemplateProcessor.ProcessTemplate<BaseRepository>(_generationOptions);
        }

        public override string BuildBaseModel()
        {
            return TemplateProcessor.ProcessTemplate<BaseModel>(_generationOptions);
        }
    }
}