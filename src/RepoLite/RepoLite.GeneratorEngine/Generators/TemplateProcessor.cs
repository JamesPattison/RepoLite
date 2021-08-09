using RepoLite.Common.Models;

namespace RepoLite.GeneratorEngine.Generators
{
    public static class TemplateProcessor
    {
        public static string ProcessTemplate<T>(Common.Options.GenerationOptions generationSettings) where T : new()
        {
            dynamic t = new T();
            t.Session = new System.Collections.Generic.Dictionary<string, object> {
                {"generationSettings", generationSettings},
            };
            t.Initialize();
            return t.TransformText();
        }
        public static string ProcessTemplate<T>(Common.Options.GenerationOptions generationSettings, Common.Models.RepositoryGenerationObject generationObject) where T : new()
        {
            dynamic t = new T();
            t.Session = new System.Collections.Generic.Dictionary<string, object> {
                {"generationSettings", generationSettings},
                {"generationObject", generationObject},
            };
            t.Initialize();
            return t.TransformText();
        }
        public static string ProcessTemplate<T>(Common.Options.GenerationOptions generationSettings, Procedure procedure) where T : new()
        {
            dynamic t = new T();
            t.Session = new System.Collections.Generic.Dictionary<string, object> {
                {"procedure", procedure},
                {"generationSettings", generationSettings},
            };
            t.Initialize();
            return t.TransformText();
        }
    }
}
