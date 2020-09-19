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


            return sb;
        }
        
        #region Abstracts
        
        
        
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
