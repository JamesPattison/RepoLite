using System;
using RepoLite.Common.Extensions;
using RepoLite.Common.Interfaces;

namespace RepoLite.Common.Models.ProcedureParameters
{
    public class BasicParameter : IProcedureParameter
    {
        public string Name { get; set; }
        public int Pos { get; set; }
        public string SqlDataType { get; set; }
        public bool IsNullable { get; set; }
        public int Length { get; set; }
        public int Precision { get; set; }
        public int Scale { get; set; }
        public string TypeString { get; set; }
        public Type Type { get; set; }
    }
}