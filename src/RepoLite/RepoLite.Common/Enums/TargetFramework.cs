using System;
using System.ComponentModel;

namespace RepoLite.Common.Enums
{
    [Serializable]
    [Flags]
    public enum TargetFramework
    {
        [Description(".NET Framework 3.5")]
        Framework35 = 1,

        [Description(".NET Framework 4")]
        Framework4 = 2,

        [Description(".NET Framework 4.5")]
        Framework45 = 4,

        [Description(".NET Framework 4.5.1")]
        Framework451 = 8,

        [Description(".NET Framework 4.5.2")]
        Framework452 = 16,

        [Description(".NET Framework 4.6")]
        Framework46 = 32,

        [Description(".NET Framework 4.6.1")]
        Framework461 = 64,

        [Description(".NET Framework 4.6.2")]
        Framework462 = 128,

        [Description(".NET Framework 4.7")]
        Framework47 = 256,

        [Description(".NET Framework 4.7.1")]
        Framework471 = 512
    }
}
