using System.Collections.Generic;
using RepoLite.Common.Interfaces;

namespace RepoLite.Common.Models.ProcedureParameters
{
    public class TemporaryTableParameter : IProcedureParameter
    {
        public List<BasicParameter> Columns { get; set; }
        public string CreateSql { get; set; }
        public string Name { get; set; }
    }
}