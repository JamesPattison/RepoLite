using RepoLite.Common.Models;
using System.Text;

namespace RepoLite.Common.Interfaces
{
    public interface ICSharpSqlServerGeneratorImports
    {
        StringBuilder GenerateRepoWrapper(Table table);
    }
}
