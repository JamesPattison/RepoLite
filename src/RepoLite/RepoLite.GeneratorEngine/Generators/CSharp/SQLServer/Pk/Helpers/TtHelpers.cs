using System;
using System.Linq;
using System.Text;
using RepoLite.Common.Models;

namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer.Pk.Helpers
{
    public class TtHelpers
    {
        public static string AppendInheritanceLogic(RepositoryGenerationObject generationObject, Func<Column, RepositoryGenerationObject, string> getInheritancelogic)
        {
            var sb = new StringBuilder();
       
            foreach (
                var column in
                generationObject.Table.Columns.Where(
                    inheritedColumn => !inheritedColumn.PrimaryKey))
            {
                sb.Append(getInheritancelogic(column, generationObject));
            }
       
            return sb.ToString();
        }
    }
}