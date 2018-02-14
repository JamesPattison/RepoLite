using RepoLite.Common.Enums;

namespace RepoLite.Models
{
    public sealed class CodePreview
    {
        public string FileName { get; set; }

        public string Content { get; set; }

        public GenerationLanguage Language { get; set; }
    }
}
