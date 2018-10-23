using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS.Base;
using NS.Models.Base;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    internal class ClassA : IBaseModel
    {
        public string EntityName => "ClassA";
        
        public string A_StringProperty { get; set; }
        public int A_IntProperty { get; set; }
        public bool A_BoolProperty { get; set; }
        public DateTime A_DateTimeProperty { get; set; }
    }

    internal class ClassB : IBaseModel
    {
        public string EntityName => "ClassB";
        public string B_StringProperty { get; set; }
        public int B_IntProperty { get; set; }
        public bool B_BoolProperty { get; set; }
        public DateTime B_DateTimeProperty { get; set; }
    }

    [TestClass]
    public class ExpressionParserTests : BaseTests
    {
        [TestMethod]
        public void TestExpressionParser_String_Equals()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_StringProperty == classB.B_StringProperty);

            var expected = "([ClassA].[A_StringProperty] = [ClassB].[B_StringProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
    }
}