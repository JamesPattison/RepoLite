using System;

namespace RepoLite.Common.Enums
{
    [Serializable]
    [Flags]
    public enum DataSourceEnum
    {
        SQLServer = 1,
        MySql = 2
    }
}
