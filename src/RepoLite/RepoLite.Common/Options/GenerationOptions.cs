using System;

namespace RepoLite.Common.Options
{
    public class GenerationOptions
    {
        public string ModelGenerationNamespace { get; set; }
        public string RepositoryGenerationNamespace { get; set; }
        public object ProcedureGenerationNamespace { get; set; }
        public string OutputDirectory { get; set; }
        public string ModelFileNameFormat { get; set; }
        public string RepositoryFileNameFormat { get; set; }
        public string ModelClassNameFormat { get; set; }
        public string RepositoryClassNameFormat { get; set; }
        public bool GenerateSealedObjects { get; set; }
        public bool IncludeCaching { get; set; }
        public bool GenerateObjectReferences { get; set; }

        public void Save()
        {
            Helpers.AddOrUpdateAppSetting("Generation:ModelGenerationNamespace", ModelGenerationNamespace);
            Helpers.AddOrUpdateAppSetting("Generation:RepositoryGenerationNamespace", RepositoryGenerationNamespace);
            Helpers.AddOrUpdateAppSetting("Generation:ProcedureGenerationNamespace", ProcedureGenerationNamespace);
            Helpers.AddOrUpdateAppSetting("Generation:OutputDirectory", OutputDirectory);
            Helpers.AddOrUpdateAppSetting("Generation:ModelFileNameFormat", ModelFileNameFormat);
            Helpers.AddOrUpdateAppSetting("Generation:RepositoryFileNameFormat", RepositoryFileNameFormat);
            Helpers.AddOrUpdateAppSetting("Generation:ModelClassNameFormat", ModelClassNameFormat);
            Helpers.AddOrUpdateAppSetting("Generation:RepositoryClassNameFormat", RepositoryClassNameFormat);
            Helpers.AddOrUpdateAppSetting("Generation:GenerateSealedObjects", GenerateSealedObjects);
            Helpers.AddOrUpdateAppSetting("Generation:IncludeCaching", IncludeCaching);
            Helpers.AddOrUpdateAppSetting("Generation:GenerateObjectReferences", GenerateObjectReferences);
        }
    }
}