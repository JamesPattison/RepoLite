namespace RepoLite.Common.Interfaces
{
    public delegate ITemplateParser TemplateParserResolver();
    public interface ITemplateParser
    {
        string BuildBaseRepository();
        string BuildBaseModel();
    }
}
