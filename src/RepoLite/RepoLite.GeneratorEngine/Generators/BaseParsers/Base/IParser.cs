using RepoLite.Common.Enums;

namespace RepoLite.GeneratorEngine.Generators.BaseParsers.Base
{
    public delegate IParser ParserResolver(DataSourceEnum datasource, GenerationLanguage generationLanguage);
    public interface IParser
    {
        string BuildBaseRepository();
        string BuildBaseModel();
    }
}
