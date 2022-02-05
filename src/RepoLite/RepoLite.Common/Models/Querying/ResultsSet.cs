using System.Collections.Generic;

namespace RepoLite.Common.Models.Querying
{
    public class ResultsSet
    {
        /// <summary>
        /// Set by Generation UI
        /// </summary>
        public string Name { get; set; }
        
        public List<ResultsSetValue> Values { get; set; }
    }
}