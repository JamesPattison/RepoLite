using RepoLite.Common.Models;
using System.Collections.Generic;
using System.Text;
using RepoLite.Common.Enums;

namespace RepoLite.GeneratorEngine
{
    public delegate IGenerator GeneratorResolver(DataSourceEnum datasource, GenerationLanguage generationLanguage);
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
