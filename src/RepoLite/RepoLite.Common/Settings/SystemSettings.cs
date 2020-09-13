using RepoLite.Common.Enums;

namespace RepoLite.Common.Settings
{
    public class SystemSettings
    {
        public string ConnectionString { get; set; }
        public int SessionDuration { get; set; }
        public DataSourceEnum DataSource { get; set; }
        public GenerationLanguage GenerationLanguage { get; set; }
    }
}
