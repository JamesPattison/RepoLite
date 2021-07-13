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
        /// <returns></returns>
        string ModelForTable(RepositoryGenerationObject generationObject);

        /// <summary>
        /// Generates a repository class
        /// </summary>
        /// <returns></returns>
        string RepositoryForTable(RepositoryGenerationObject generationObject);

        /// <summary>
        /// Gets the file extension for the model
        /// </summary>
        /// <returns></returns>
        string FileExtension();
        
        string BuildBaseRepository();
        
        string BuildBaseModel();
    }
}
