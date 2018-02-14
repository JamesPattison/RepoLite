namespace RepoLite.Common.Models
{
    public class TableAndSchema
    {
        public string Table { get; set; }
        public string Schema { get; set; }

        public TableAndSchema() { }
        public TableAndSchema(string schema, string table)
        {
            Schema = schema;
            Table = table;
        }
    }
}
