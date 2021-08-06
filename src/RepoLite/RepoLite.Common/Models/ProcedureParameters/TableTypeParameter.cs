using System.Collections.Generic;
using RepoLite.Common.Interfaces;

namespace RepoLite.Common.Models.ProcedureParameters
{
    public class TableTypeParameter : IProcedureParameter
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public string SqlName { get; set; }
        public List<BasicParameter> Columns { get; set; }
    }
}