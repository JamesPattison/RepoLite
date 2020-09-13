namespace RepoLite.GeneratorEngine.Generators.BaseParsers.Base
{
    public delegate IParser ParserResolver();
    public interface IParser
    {
        string BuildBaseRepository();
        string BuildBaseModel();
    }
}
