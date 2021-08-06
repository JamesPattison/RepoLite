using RepoLite.Common.Models.ProcedureParameters;

namespace RepoLite.Common.Models.Querying
{
    public class ProcedureParameterInfo: BasicParameter
    {
        public bool IsOutput { get; set; }
        public bool IsUserDefined { get; set; }
        public bool IsTableType { get; set; }
        public string UserDefinedSchema { get; set; }
        public string UserDefinedName { get; set; }
    }
}