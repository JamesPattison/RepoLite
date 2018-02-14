using RepoLite.Common.Models;
using System.Text;

namespace RepoLite.GeneratorEngine
{
    public interface IGenerator
    {
        /// <summary>
        /// Generates a model class
        /// </summary>
        /// <param name="table">The table object to use</param>
        /// <returns></returns>
        StringBuilder ModelForTable(Table table);

        /// <summary>
        /// Generates a repository class
        /// </summary>
        /// <param name="table">The table object to use</param>
        /// <returns></returns>
        StringBuilder RepositoryForTable(Table table);

        /// <summary>
        /// Gets the file extension for the model
        /// </summary>
        /// <returns></returns>
        string FileExtension();
    }
}
