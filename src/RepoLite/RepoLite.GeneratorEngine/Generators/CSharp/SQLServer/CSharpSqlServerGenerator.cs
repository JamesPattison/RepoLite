using Microsoft.Extensions.Options;
using RepoLite.Common.Models;
using RepoLite.Common.Options;
using System.Linq;
using RepoLite.GeneratorEngine.Generators.CSharp.SQLServer.Templates;
using RepoLite.GeneratorEngine.Generators.CSharp.SQLServer.Templates.Repositories.Pk;

namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer
{
    public class CSharpSqlServerGenerator : CSharpCodeGenerator
    {
        private GenerationOptions _generationOptions;

        public CSharpSqlServerGenerator(IOptions<GenerationOptions> generationOptions)
        {
            _generationOptions = generationOptions.Value;
        }
        public override string BuildModel(RepositoryGenerationObject generationObject)
        {
            if (generationObject.Table.PrimaryKeys.Any())
            {
                if (generationObject.Table.PrimaryKeys.Count == 1)
                {
                    return TemplateProcessor.ProcessTemplate<Model>(_generationOptions, generationObject);
                }
                else
                {
                    //todo
                }
            }
            else
            {
                //todo
            }
            
            return string.Empty;
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
                    //todo
                }
            }
            else
            {
                //todo
            }

            return string.Empty;
        }

        public override string BuildProcedure(ProcedureGenerationObject procedureGenerationObject)
        {
            return TemplateProcessor.ProcessTemplate<Procedure>(_generationOptions, procedureGenerationObject);
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
