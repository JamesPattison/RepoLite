using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RepoLite.Common.Extensions;
using RepoLite.Common.Interfaces;
using RepoLite.Common.Models;
using RepoLite.Common.Settings;

namespace RepoLite.Generator.DotNet.Generators.Base
{
    internal abstract class BaseGenerator : IGenerator
    {
        public static string Tab1 = "\t";
        public static string Tab2 = "\t\t";
        public static string Tab3 = "\t\t\t";
        public static string Tab4 = "\t\t\t\t";
        public static string Tab5 = "\t\t\t\t\t";
        public static string Tab6 = "\t\t\t\t\t\t";
        public static string Tab7 = "\t\t\t\t\t\t\t";
        public static string Tab8 = "\t\t\t\t\t\t\t\t";
        public static string Tab9 = "\t\t\t\t\t\t\t\t\t";

        public static bool IsCSharpNullable(string type)
        {
            return type != "byte[]" && type != "object" && type != "string" && type != "XmlDocument" &&
                   type != "Byte[]" && type != "Object" && type != "String";
        }
        
        protected readonly Table _table;
        private readonly GenerationSettings _generationSettings;

        public BaseGenerator(
            IOptions<GenerationSettings> generationSettings,
            Table table)
        {
            _table = table;
            _generationSettings = generationSettings.Value;
        }

        public string FileExtension()
        {
            return "cs";
        }

        protected internal string RepositoryName(string repository)
        {
            return repository.ToRepositoryName(_generationSettings.RepositoryClassNameFormat);
        }

        protected internal string ModelName(string model)
        {
            return model.ToModelName(_generationSettings.ModelClassNameFormat);
        }
        
        public StringBuilder ModelForTable(Table table, List<Table> otherTables)
        {
            var sb = new StringBuilder();



            return sb;
        }

        public StringBuilder RepositoryForTable(Table table, List<Table> otherTables)
        {
            var sb = new StringBuilder();

            AppendImports(sb);
            Interface(sb);

            return sb;
        }
        
        #region Abstracts

        public abstract void Interface(StringBuilder sb);
        
        #endregion
        
        #region Helpers

        private void AppendImports(StringBuilder sb)
        {
            sb.AppendLine($"using {_generationSettings.RepositoryGenerationNamespace}.Base;");
            sb.AppendLine($"using {_generationSettings.ModelGenerationNamespace};");
            sb.AppendLine($"using {_generationSettings.ModelGenerationNamespace}.Base;");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Data;");
            sb.AppendLine("using System.Data.SqlClient;");
            sb.AppendLine("using System.IO;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Xml;");
        }

        #endregion
    }
}
