using FuzzyString;
using RepoLite.Common;
using RepoLite.Common.Enums;
using RepoLite.Common.Models;
using RepoLite.GeneratorEngine.Generators;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using RepoLite.Common.Options;

namespace RepoLite.GeneratorEngine
{
    public abstract class CodeGenerator : IGenerator
    {
        public abstract StringBuilder ModelForTable(Table table, List<Table> otherTables);
        public abstract StringBuilder RepositoryForTable(Table table, List<Table> otherTables);

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

        public abstract string FileExtension();
    }
}
