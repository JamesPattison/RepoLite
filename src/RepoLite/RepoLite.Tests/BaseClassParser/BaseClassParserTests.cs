using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoLite.GeneratorEngine.Generators.BaseParsers.Base;
using System.Collections.Generic;

namespace RepoLite.Tests.BaseClassParser
{
    public class TestBaseClassParser : GeneratorEngine.Generators.BaseParsers.Base.BaseClassParser
    {
        public TestBaseClassParser(List<string> validKeys) : base(validKeys)
        {
        }

        public TestBaseClassParser(List<string> validKeys, BaseClassParseOptions parseOptions) : base(validKeys, parseOptions)
        {
        }

        public override string BuildBaseRepository()
        {
            return string.Empty;
        }

        public override string BuildBaseModel()
        {
            return string.Empty;
        }
    }

    [TestClass]
    public class BaseClassParserTests
    {
        [TestMethod]
        public void TestOneParam_Provided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check

text inside ifcheck";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestOneParam_NotProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check
";

            var parser = new TestBaseClassParser(new List<string>());

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_Provided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@//IFCHECK@@@

@@@ANOTHERCHECK@@@
hello
line
another line
@@@//ANOTHERCHECK@@@";

            var expected =
                @"string outside of check

text inside ifcheck

hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_OneProvided_OneNotProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@//IFCHECK@@@

@@@ANOTHERCHECK@@@
hello
line
another line
@@@//ANOTHERCHECK@@@";

            var expected =
                @"string outside of check


hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_OneProvided_OneNotProvided_RemoveMultipleBlankLines()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@//IFCHECK@@@

@@@ANOTHERCHECK@@@
hello
line
another line
@@@//ANOTHERCHECK@@@";

            var expected =
                @"string outside of check

hello
line
another line";

            var parseOptions = new BaseClassParseOptions
            {
                RemoveMultipleBlankLines = true
            };

            var parser = new TestBaseClassParser(new List<string> { "ANOTHERCHECK" }, parseOptions);

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_Nested_BothProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@ANOTHERCHECK@@@
hello
line
another line
@@@//ANOTHERCHECK@@@
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check

text inside ifcheck
hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_Nested_InnerProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@ANOTHERCHECK@@@
hello
line
another line
@@@//ANOTHERCHECK@@@
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check
";

            var parser = new TestBaseClassParser(new List<string> { "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_Nested_OuterProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifcheck
@@@ANOTHERCHECK@@@
hello
line
another line
@@@//ANOTHERCHECK@@@
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check

text inside ifcheck";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_OrJoined_OneProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK || ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK || ANOTHERCHECK@@@";

            var expected =
                @"string outside of check

text inside ifelsecheck
hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_AndJoined_OneProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK && ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK && ANOTHERCHECK@@@";

            var expected =
                @"string outside of check
";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestTwoParam_AndJoined_BothProvided()
        {
            var template =
                @"string outside of check

@@@IFCHECK && ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK && ANOTHERCHECK@@@";

            var expected =
                @"string outside of check

text inside ifelsecheck
hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestThreeParam_OneNestedAndJoin_AllProvided()
        {
            var template =
                @"string outside of check
@@@SUPERCHECK@@@
secret code
@@@IFCHECK && ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK && ANOTHERCHECK@@@
@@@//SUPERCHECK@@@";

            var expected =
                @"string outside of check
secret code
text inside ifelsecheck
hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "SUPERCHECK", "IFCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestThreeParam_OneNestedOrJoin_TwoProvidedProvided()
        {
            var template =
                @"string outside of check
@@@SUPERCHECK@@@
secret code
@@@IFCHECK || ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK || ANOTHERCHECK@@@
@@@//SUPERCHECK@@@";

            var expected =
                @"string outside of check
secret code
text inside ifelsecheck
hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "SUPERCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestThreeParam_OneNestedAndJoin_TwoProvided()
        {
            var template =
                @"string outside of check
@@@SUPERCHECK@@@
secret code
@@@IFCHECK && ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK && ANOTHERCHECK@@@
@@@//SUPERCHECK@@@";

            var expected =
                @"string outside of check
secret code";

            var parser = new TestBaseClassParser(new List<string> { "SUPERCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestThreeParam_OneNestedAndJoin_OuterNotProvided()
        {
            var template =
                @"string outside of check
@@@SUPERCHECK@@@
secret code
@@@IFCHECK && ANOTHERCHECK@@@
text inside ifelsecheck
hello
line
another line
@@@//IFCHECK && ANOTHERCHECK@@@
@@@//SUPERCHECK@@@";

            var expected =
                @"string outside of check";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK", "ANOTHERCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestSixParam_SeveralNestedAndJoins_SomeProvided()
        {
            var template =
                @"string outside of check
@@@SUPERCHECK && HEROCHECK@@@
secret code
@@@IFCHECK && ANOTHERCHECK@@@
text inside ifelsecheck
hello
@@@INNERCHECK && SHAKEITCHECK@@@
line
@@@//INNERCHECK && SHAKEITCHECK@@@
another line
@@@//IFCHECK && ANOTHERCHECK@@@
@@@//SUPERCHECK && HEROCHECK@@@";

            var expected =
                @"string outside of check
secret code
text inside ifelsecheck
hello
another line";

            var parser = new TestBaseClassParser(new List<string> { "SUPERCHECK", "HEROCHECK", "IFCHECK", "ANOTHERCHECK", "SHAKEITCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestSixParam_SeveralNestedOrJoins_SomeProvided()
        {
            var template =
                @"string outside of check
@@@SUPERCHECK || HEROCHECK@@@
secret code
@@@IFCHECK || ANOTHERCHECK@@@
text inside ifelsecheck
hello
@@@INNERCHECK || SHAKEITCHECK@@@
line
@@@//INNERCHECK || SHAKEITCHECK@@@
another line
@@@//IFCHECK || ANOTHERCHECK@@@
@@@//SUPERCHECK || HEROCHECK@@@";

            var expected =
                @"string outside of check
secret code
text inside ifelsecheck
hello
line
another line";

            var parser = new TestBaseClassParser(new List<string> { "SUPERCHECK", "IFCHECK", "INNERCHECK", "SHAKEITCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestOneParam_IfElse()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifelsecheck
hello
line
@@@ELSE [IFCHECK]@@@
another line
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check

text inside ifelsecheck
hello
line";

            var parser = new TestBaseClassParser(new List<string> { "IFCHECK" });

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }

        [TestMethod]
        public void TestOneParam_IfElseIfElse()
        {
            var template =
                @"string outside of check

@@@IFCHECK@@@
text inside ifelsecheck
hello
line
@@@ELSE [IFCHECK]@@@
@@@ANOTHERCHECK@@@
another line
@@@//ANOTHERCHECK@@@
last line
@@@//IFCHECK@@@";

            var expected =
                @"string outside of check

last line";

            var parser = new TestBaseClassParser(new List<string>());

            var actual = parser.Parse(template);

            Assert.IsTrue(actual == expected, $"received: {actual}");
        }
    }
}
