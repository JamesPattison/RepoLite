using System.Text;
using RepoLite.Common.Models;

namespace RepoLite.GeneratorEngine.Generators
{
    public class VBSqlServerGenerator: CodeGenerator
    {
        public override StringBuilder ModelForTable(Table table)
        {
            throw new System.NotImplementedException();
        }

        public override StringBuilder RepositoryForTable(Table table)
        {
            throw new System.NotImplementedException();
        }

        public override string FileExtension()
        {
            throw new System.NotImplementedException();
        }
    }
}