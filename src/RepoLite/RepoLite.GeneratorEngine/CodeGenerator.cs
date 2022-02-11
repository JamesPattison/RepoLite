using FuzzyString;
using RepoLite.Common.Models;
using System.Collections.Generic;
using System.Text;

namespace RepoLite.GeneratorEngine
{
    public abstract class CodeGenerator : IGenerator
    {
        public abstract string BuildModel(RepositoryGenerationObject generationObject);
        public abstract string BuildRepository(RepositoryGenerationObject generationObject);
        public abstract string BuildProcedure(ProcedureGenerationObject procedureGenerationObject);

        public abstract string FileExtension();
        public abstract string BuildBaseRepository();
        public abstract string BuildBaseModel();
        
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
    }
}
