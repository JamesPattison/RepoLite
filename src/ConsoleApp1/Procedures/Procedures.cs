namespace Biomind.Data.Procedures
{
    public partial class Procedures
    {
        protected string _connectionString;
        
        public Procedures(string connectionString)
        {
            _connectionString = connectionString;
        }
    }
}