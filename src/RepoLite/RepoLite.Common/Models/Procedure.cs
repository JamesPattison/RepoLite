using System;

namespace RepoLite.Common.Models
{
    public class Procedure
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public Type ReturnType { get; set; }
    }
}
