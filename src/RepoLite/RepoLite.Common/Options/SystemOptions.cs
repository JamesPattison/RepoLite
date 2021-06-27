using RepoLite.Common.Enums;

namespace RepoLite.Common.Options
{
    public class SystemOptions
    {
        public string ConnectionString { get; set; }
        public DataSourceEnum DataSource { get; set; }
        public GenerationLanguage GenerationLanguage { get; set; }
    }
}