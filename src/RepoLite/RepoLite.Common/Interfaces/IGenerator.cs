using RepoLite.Common.Models;
using System.Collections.Generic;
using System.Text;

namespace RepoLite.Common.Interfaces
{
    public delegate IGenerator GeneratorResolver();
    public interface IGenerator
    {
        /// <summary>
        /// Generates a model class
        /// </summary>
        /// <param name="table">The table object to use</param>
        /// <returns></returns>
        StringBuilder ModelForTable(Table table, List<Table> otherTables);

        /// <summary>
        /// Generates a repository class
        /// </summary>
        /// <param name="table">The table object to use</param>
        /// <returns></returns>
        StringBuilder RepositoryForTable(Table table, List<Table> otherTables);

        /// <summary>
        /// Gets the file extension for the model
        /// </summary>
        /// <returns></returns>
        string FileExtension();
    }
}
