namespace RepoLite.Common.Models
{
    public class NameAndSchema
    {
        public string Name { get; set; }
        public string Schema { get; set; }

        public NameAndSchema() { }
        public NameAndSchema(string schema, string name)
        {
            Schema = schema;
            Name = name;
        }
    }
}
