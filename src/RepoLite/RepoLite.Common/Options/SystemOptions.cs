using RepoLite.Common.Enums;
using System;

namespace RepoLite.Common.Options
{
    public class SystemOptions
    {
        public string ConnectionString { get; set; }
        public DataSourceEnum DataSource { get; set; }
        public GenerationLanguage GenerationLanguage { get; set; }

        public void Save()
        {
            Helpers.AddOrUpdateAppSetting("System:ConnectionString", ConnectionString);
            Helpers.AddOrUpdateAppSetting("System:DataSource", DataSource);
            Helpers.AddOrUpdateAppSetting("System:GenerationLanguage", GenerationLanguage);
        }
    }
}