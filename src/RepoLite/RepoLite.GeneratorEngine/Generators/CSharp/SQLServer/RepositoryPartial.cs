namespace RepoLite.GeneratorEngine.Generators.CSharp.SQLServer
{
    public partial class Repository
    {
        private readonly RepositoryGenerationObject sqlQuery;

        public Repository(RepositoryGenerationObject sqlQuery)
        {
            this.sqlQuery = sqlQuery;
        }
    }
}