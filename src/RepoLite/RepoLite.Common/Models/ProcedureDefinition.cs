namespace RepoLite.Common.Models
{
    public class ProcedureDefinition : NameAndSchema
    {
        public string Definition { get; set; }

        public ProcedureDefinition(string schema, string name, string definition)
            : base(schema, name)
        {
            Definition = definition;
        }
    }
}