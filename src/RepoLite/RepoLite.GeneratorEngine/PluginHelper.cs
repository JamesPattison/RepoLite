using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Interfaces;
using RepoLite.Plugins.K3;

namespace RepoLite.GeneratorEngine
{
    public static class PluginHelper
    {
        public static ICSharpSqlServerGeneratorImports GetPlugin()
        {
            switch (AppSettings.Generation.Plugin)
            {
                case PluginEnum.K3:
                    return new CSharpSqlServerGeneratorImports();

                case PluginEnum.None:
                default:
                    return null;
            }
        }
    }
}
