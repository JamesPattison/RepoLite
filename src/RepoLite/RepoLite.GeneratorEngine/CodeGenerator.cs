using FuzzyString;
using RepoLite.Common.Models;
using System.Collections.Generic;
using System.Text;

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

        //public static IGenerator GetGenerator(GenerationLanguage generationLanguage, DataSourceEnum dataSource)
        //{
        //    IGenerator generator = null;
        //    switch (generationLanguage)
        //    {
        //        case GenerationLanguage.CSharp:
        //            switch (dataSource)
        //            {
        //                case DataSourceEnum.SQLServer:
        //                    generator = new CSharpSqlServerGenerator();
        //                    break;
        //            }
        //            break;
        //    }

        //    if (generator == null)
        //        throw new Exception("Generator/DataSource combo not supported yet");

        //    return generator;
        //}

        public abstract string FileExtension();
    }
}
