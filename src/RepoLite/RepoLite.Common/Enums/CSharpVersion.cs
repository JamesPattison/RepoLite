using System;

namespace RepoLite.Common.Enums
{
    [Serializable]
    [Flags]
    public enum CSharpVersion
    {
        CSharp4 = 1,
        CSharp5 = 2,
        CSharp6 = 4,
        CSharp7 = 8,
        CSharp71 = 16,
        CSharp72 = 32
    }
}
