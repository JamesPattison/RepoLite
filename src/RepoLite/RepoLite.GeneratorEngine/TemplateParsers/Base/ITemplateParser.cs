namespace RepoLite.GeneratorEngine.TemplateParsers.Base
{
    public delegate ITemplateParser TemplateParserResolver();
    public interface ITemplateParser
    {
        string BuildBaseRepository();
        string BuildBaseModel();
    }
}
