using System;
using System.Collections.Generic;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models.Querying;

namespace RepoLite.Common.Models
{
    public class Procedure
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        
        public List<IProcedureParameter> Parameters { get; set; }
        public List<ResultsSet> ResultSets { get; set; }
    }
}
