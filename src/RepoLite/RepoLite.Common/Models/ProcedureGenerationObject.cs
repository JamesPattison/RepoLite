using System;
using System.Collections.Generic;
using System.Linq;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models.Querying;

namespace RepoLite.Common.Models
{
    public class ProcedureGenerationObject
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        
        public List<IProcedureParameter> Parameters { get; set; }
        public List<ResultsSet> ResultSets { get; set; }
        
        public bool HasParameters => Parameters.Any();
        public bool HasResults => ResultSets.Any();
    }
}
