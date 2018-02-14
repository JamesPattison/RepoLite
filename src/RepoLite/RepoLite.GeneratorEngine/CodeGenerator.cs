using FuzzyString;
using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.GeneratorEngine.Generators;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepoLite.GeneratorEngine
{
    public abstract class CodeGenerator : IGenerator
    {
        public abstract StringBuilder ModelForTable(Table table);
        public abstract StringBuilder RepositoryForTable(Table table);

        protected bool AreWordsSimilar(string first, string second)
        {
            var options = new List<FuzzyStringComparisonOptions>
            {
                FuzzyStringComparisonOptions.UseOverlapCoefficient,
                FuzzyStringComparisonOptions.UseLongestCommonSubsequence,
                FuzzyStringComparisonOptions.UseLongestCommonSubstring,
                FuzzyStringComparisonOptions.CaseSensitive,
                FuzzyStringComparisonOptions.UseJaroWinklerDistance
            };

            return first.ApproximatelyEquals(second, options, FuzzyStringComparisonTolerance.Normal);
        }

        public static IGenerator GetGenerator()
        {
            IGenerator generator = null;
            switch (AppSettings.System.GenerationLanguage)
            {
                case GenerationLanguage.CSharp:
                    switch (AppSettings.System.DataSource)
                    {
                        case DataSourceEnum.SQLServer:
                            generator = new CSharpSqlServerGenerator();
                            break;
                    }
                    break;
            }

            if (generator == null)
                throw new Exception("Generator/DataSource combo not supported yet");

            return generator;
        }

        public abstract string FileExtension();
    }
}
