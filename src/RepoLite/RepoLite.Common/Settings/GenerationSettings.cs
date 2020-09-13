namespace RepoLite.Common.Settings
{
    public class GenerationSettings
    {
        public string ModelGenerationNamespace { get; set; }
        public string RepositoryGenerationNamespace { get; set; }
        public string OutputDirectory { get; set; }
        public string ModelFileNameFormat { get; set; }
        public string RepositoryFileNameFormat { get; set; }
        public string ModelClassNameFormat { get; set; }
        public string RepositoryClassNameFormat { get; set; }
        public bool GenerateSealedObjects { get; set; }
    }
}
