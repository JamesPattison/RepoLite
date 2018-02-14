using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoLite.Common.Enums;
using RepoLite.GeneratorEngine.Generators.BaseParsers;

namespace RepoLite.Tests.BaseClassParser
{
    [TestClass]
    public class CSharpSqlServerBaseClassParserTests
    {
        [TestMethod]
        public void TestCSharpSqlRepo_CallerMemberName_Framework35()
        {
            var template =
                @"@@@CSharp3@@@
        protected void SetValue<T>(ref T prop, T value, string propName)
@@@ELSE [CSharp3]@@@
@@@Framework45 || Framework451 || Framework452 || Framework46 || Framework461 || Framework462 || Framework47 || Framework471@@@
        protected void SetValue<T>(ref T prop, T value, [CallerMemberName] string propName = "")
@@@//Framework45 || Framework451 || Framework452 || Framework46 || Framework461 || Framework462 || Framework47 || Framework471@@@
        protected void SetValue<T>(ref T prop, T value, string propName = "")
@@@//CSharp3@@@";

            var expected =
                @"        protected void SetValue<T>(ref T prop, T value, string propName = "")";

            var parser = new CSharpSqlServerBaseClassParser(TargetFramework.Framework35, CSharpVersion.CSharp6);

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestCSharpSqlRepo_CallerMemberName_CSharp3()
        {
            var template =
                @"@@@CSharp4@@@
        protected void SetValue<T>(ref T prop, T value, string propName)
@@@ELSE [CSharp4]@@@
@@@Framework45 || Framework451 || Framework452 || Framework46 || Framework461 || Framework462 || Framework47 || Framework471@@@
        protected void SetValue<T>(ref T prop, T value, [CallerMemberName] string propName = "")
@@@//Framework45 || Framework451 || Framework452 || Framework46 || Framework461 || Framework462 || Framework47 || Framework471@@@
        protected void SetValue<T>(ref T prop, T value, string propName = "")
@@@//CSharp4@@@";

            var expected =
                @"        protected void SetValue<T>(ref T prop, T value, string propName)";

            var parser = new CSharpSqlServerBaseClassParser(TargetFramework.Framework35, CSharpVersion.CSharp4);

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestCSharpSqlRepo_ComplexNested()
        {
            var template =
                @"@@@Framework1 || Framework11 || Framework2 || Framework3@@@
@@@CSharp3 || CSharp4 || CSharp5@@@
                        var sb = new StringBuilder();
                        for (var j = i * batchSize; j < (i * batchSize) + batchSize; j++)
                        {
                            sb.Append(GetTypeVal(col, enumerable[j]));
                        }
                        result = sb.ToString().TrimEnd(' ').TrimEnd(',');
@@@ELSE [CSharp3 || CSharp4 || CSharp5]@@@
                        var sb = new StringBuilder();
                        for (var j = i * batchSize; j < (i * batchSize) + batchSize; j++)
                        {
                            sb.Append(GetTypeVal(col, enumerable[j]));
                        }
                        result = sb.ToString().TrimEnd(' ').TrimEnd(',');

@@@//CSharp3 || CSharp4 || CSharp5@@@
@@@ELSE [Framework1 || Framework11 || Framework2 || Framework3]@@@
                        result = enumerable
                            .Skip(i * batchSize)
                            .Take(batchSize)
@@@CSharp3 || CSharp4 || CSharp5@@@
                            .Aggregate(result, (current, o) => current + GetTypeVal(col, o) + "", "")
@@@ELSE [CSharp3 || CSharp4 || CSharp5]@@@
                .Aggregate(result, (current, o) => current + $""{GetTypeVal(col, o)}, "")
@@@//CSharp3 || CSharp4 || CSharp5@@@
                .TrimEnd(' ')
                .TrimEnd(',');

@@@//Framework1 || Framework11 || Framework2 || Framework3@@@";

            var expected =
                @"                        result = enumerable
                            .Skip(i * batchSize)
                            .Take(batchSize)
                            .Aggregate(result, (current, o) => current + GetTypeVal(col, o) + "", "")
                .TrimEnd(' ')
                .TrimEnd(',');
";

            var parser = new CSharpSqlServerBaseClassParser(TargetFramework.Framework35, CSharpVersion.CSharp4);

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestCSharpSqlRepo_ComplexNested_1()
        {
            var template =
                @"@@@Framework1 || Framework11 || Framework2 || Framework3@@@
                    var objList = new List<object>();
                    foreach (var obj in (val as IEnumerable))
                    {
                        objList.Add(obj);
                    }

                    var enumerable = objList.ToArray();
@@@ELSE [Framework1 || Framework11 || Framework2 || Framework3]@@@
@@@CSharp7 || CSharp71 || CSharp72@@@
                    var enumerable = list as object[] ?? list.Cast<object>().ToArray();
@@@ELSE [CSharp7 || CSharp71 || CSharp72]@@@
                    var enumerable = val as object[] ?? (val as IEnumerable).Cast<object>().ToArray();
@@@//CSharp7 || CSharp71 || CSharp72@@@
@@@//Framework1 || Framework11 || Framework2 || Framework3@@@";

            var expected =
                @"                    var enumerable = list as object[] ?? list.Cast<object>().ToArray();";

            var parser = new CSharpSqlServerBaseClassParser(TargetFramework.Framework35, CSharpVersion.CSharp71);

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestCSharpSqlRepo_ComplexNested_1_1()
        {
            var template =
                @"@@@Framework1 || Framework11 || Framework2 || Framework3 || Framework35@@@
                    var objList = new List<object>();
                    foreach (var obj in (val as IEnumerable))
                    {
                        objList.Add(obj);
                    }

                    var enumerable = objList.ToArray();
@@@ELSE [Framework1 || Framework11 || Framework2 || Framework3 || Framework35]@@@
@@@CSharp7 || CSharp71 || CSharp72@@@
                    var enumerable = list as object[] ?? list.Cast<object>().ToArray();
@@@ELSE [CSharp7 || CSharp71 || CSharp72]@@@
                    var enumerable = val as object[] ?? (val as IEnumerable).Cast<object>().ToArray();
@@@//CSharp7 || CSharp71 || CSharp72@@@
@@@//Framework1 || Framework11 || Framework2 || Framework3 || Framework35@@@";

            var expected =
                @"                    var objList = new List<object>();
                    foreach (var obj in (val as IEnumerable))
                    {
                        objList.Add(obj);
                    }

                    var enumerable = objList.ToArray();";

            var parser = new CSharpSqlServerBaseClassParser(TargetFramework.Framework35, CSharpVersion.CSharp5);

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }
    }
}
