using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS.Base;
using NS.Models.Base;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    internal class ClassA : IBaseModel
    {
        public string EntityName => "ClassA";

        public IBaseModel SetValues(DataRow row, string propertyPrefix)
        {
            return this;
        }

        public string A_StringProperty { get; set; }
        public int A_IntProperty { get; set; }
        public bool A_BoolProperty { get; set; }
        public DateTime A_DateTimeProperty { get; set; }
    }

    internal class ClassB : IBaseModel
    {
        public string EntityName => "ClassB";

        public IBaseModel SetValues(DataRow row, string propertyPrefix)
        {
            return this;
        }

        public string B_StringProperty { get; set; }
        public int B_IntProperty { get; set; }
        public bool B_BoolProperty { get; set; }
        public DateTime B_DateTimeProperty { get; set; }
    }

    internal class ClassC : IBaseModel
    {
        public string EntityName => "ClassC";

        public IBaseModel SetValues(DataRow row, string propertyPrefix)
        {
            return this;
        }

        public int? B_Ref { get; set; }
    }

    [TestClass]
    public class ExpressionParserTests : BaseTests
    {
        private Dictionary<string, string> _keyMap = new Dictionary<string, string>
        {
            {"classA", "c0"},
            {"classB", "c1"}
        };

        #region [String] 

        [TestMethod]
        public void TestExpressionParser_String_Equals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_StringProperty == classB.B_StringProperty, _keyMap);

            var expected = "([c0].[A_StringProperty] = [c1].[B_StringProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotEquals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_StringProperty != classB.B_StringProperty, _keyMap);

            var expected = "([c0].[A_StringProperty] <> [c1].[B_StringProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_Contains_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_StringProperty.Contains(classB.B_StringProperty), _keyMap);

            var expected = "([c0].[A_StringProperty] LIKE '%'+[c1].[B_StringProperty]+'%')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotContains_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => !classA.A_StringProperty.Contains(classB.B_StringProperty), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] LIKE '%'+[c1].[B_StringProperty]+'%'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_StartsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_StringProperty.StartsWith(classB.B_StringProperty), _keyMap);

            var expected = "([c0].[A_StringProperty] LIKE [c1].[B_StringProperty]+'%')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotStartsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => !classA.A_StringProperty.StartsWith(classB.B_StringProperty), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] LIKE [c1].[B_StringProperty]+'%'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_EndsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_StringProperty.EndsWith(classB.B_StringProperty), _keyMap);

            var expected = "([c0].[A_StringProperty] LIKE '%'+[c1].[B_StringProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotEndsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => !classA.A_StringProperty.EndsWith(classB.B_StringProperty), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] LIKE '%'+[c1].[B_StringProperty]))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_Equals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty == "hello", _keyMap);

            var expected = "([c0].[A_StringProperty] = 'hello')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotEquals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty != "hello", _keyMap);

            var expected = "([c0].[A_StringProperty] <> 'hello')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_Contains_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty.Contains("hello"), _keyMap);

            var expected = "([c0].[A_StringProperty] LIKE '%hello%')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotContains_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_StringProperty.Contains("hello"), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] LIKE '%hello%'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_StartsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty.StartsWith("hello"), _keyMap);

            var expected = "([c0].[A_StringProperty] LIKE 'hello%')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotStartsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_StringProperty.StartsWith("hello"), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] LIKE 'hello%'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_EndsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty.EndsWith("hello"), _keyMap);

            var expected = "([c0].[A_StringProperty] LIKE '%hello')";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotEndsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_StringProperty.EndsWith("hello"), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] LIKE '%hello'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_In_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(
                classA => new[] {"one", "two", "three"}.Contains(classA.A_StringProperty), _keyMap);

            var expected = "([c0].[A_StringProperty] IN ('one','two','three'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_String_NotIn_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(
                classA => !new[] {"one", "two", "three"}.Contains(classA.A_StringProperty), _keyMap);

            var expected = "(NOT ([c0].[A_StringProperty] IN ('one','two','three')))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion

        #region [Int]

        [TestMethod]
        public void TestExpressionParser_Int_Equals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_IntProperty == classB.B_IntProperty, _keyMap);

            var expected = "([c0].[A_IntProperty] = [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_NotEquals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_IntProperty != classB.B_IntProperty, _keyMap);

            var expected = "([c0].[A_IntProperty] <> [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_LessThan_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_IntProperty < classB.B_IntProperty, _keyMap);

            var expected = "([c0].[A_IntProperty] < [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_LessThanEqual_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_IntProperty <= classB.B_IntProperty, _keyMap);

            var expected = "([c0].[A_IntProperty] <= [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_GreaterThan_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_IntProperty > classB.B_IntProperty, _keyMap);

            var expected = "([c0].[A_IntProperty] > [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_GreaterThanEqual_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_IntProperty >= classB.B_IntProperty, _keyMap);

            var expected = "([c0].[A_IntProperty] >= [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_Mod()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => (classA.A_IntProperty % classB.B_IntProperty) == 1, _keyMap);

            var expected = "(([c0].[A_IntProperty] % [c1].[B_IntProperty]) = 1)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_Mod_2()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => (classA.A_IntProperty % classB.B_IntProperty) ==
                                    (classB.B_IntProperty % classA.A_IntProperty), _keyMap);

            var expected =
                "(([c0].[A_IntProperty] % [c1].[B_IntProperty]) = ([c1].[B_IntProperty] % [c0].[A_IntProperty]))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_Equals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty == 3, _keyMap);

            var expected = "([c0].[A_IntProperty] = 3)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_NotEquals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty != 4, _keyMap);

            var expected = "([c0].[A_IntProperty] <> 4)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_LessThan_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty < 5, _keyMap);

            var expected = "([c0].[A_IntProperty] < 5)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_LessThanEqual_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty <= 5, _keyMap);

            var expected = "([c0].[A_IntProperty] <= 5)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_GreaterThan_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty > 10, _keyMap);

            var expected = "([c0].[A_IntProperty] > 10)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_GreaterThanEqual_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty >= 10, _keyMap);

            var expected = "([c0].[A_IntProperty] >= 10)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_In_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => new[] {1, 2, 3}.Contains(classA.A_IntProperty), _keyMap);

            var expected = "([c0].[A_IntProperty] IN ('1','2','3'))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Int_NotIn_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !new[] {1, 2, 3}.Contains(classA.A_IntProperty),
                _keyMap);

            var expected = "(NOT ([c0].[A_IntProperty] IN ('1','2','3')))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion

        #region [Bool]

        [TestMethod]
        public void TestExpressionParser_Bool_True_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_BoolProperty == classB.B_BoolProperty, _keyMap);

            var expected = "([c0].[A_BoolProperty] = [c1].[B_BoolProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Bool_True_Col_2()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_BoolProperty == classB.B_BoolProperty && !classB.B_BoolProperty, _keyMap);

            var expected = "(([c0].[A_BoolProperty] = [c1].[B_BoolProperty]) AND (NOT ([c1].[B_BoolProperty] = 1)))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Bool_False_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_BoolProperty != classB.B_BoolProperty, _keyMap);

            var expected = "([c0].[A_BoolProperty] <> [c1].[B_BoolProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Bool_True_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_BoolProperty, _keyMap);

            var expected = "([c0].[A_BoolProperty] = 1)";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_Bool_False_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_BoolProperty, _keyMap);

            var expected = "(NOT ([c0].[A_BoolProperty] = 1))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion

        #region [DateTime]

        [TestMethod]
        public void TestExpressionParser_DateTime_LessThan()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_DateTimeProperty < classB.B_DateTimeProperty, _keyMap);

            var expected = "([c0].[A_DateTimeProperty] < [c1].[B_DateTimeProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_DateTime_LessThanEqualTo()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>(
                (classA, classB) => classA.A_DateTimeProperty <= classB.B_DateTimeProperty, _keyMap);

            var expected = "([c0].[A_DateTimeProperty] <= [c1].[B_DateTimeProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion

        [TestMethod]
        public void TestNullableJoin()
        {
            var sql = ExpressionParser.ToSql<ClassC, ClassB>((classA, classB) => classA.B_Ref == classB.B_IntProperty,
                _keyMap);

            var expected = "([c0].[B_Ref] = [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestNullableJoin_Value()
        {
            var sql = ExpressionParser.ToSql<ClassC, ClassB>(
                (classA, classB) => classA.B_Ref.Value == classB.B_IntProperty, _keyMap);

            var expected = "([c0].[B_Ref] = [c1].[B_IntProperty])";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestNullableJoin_HasValue()
        {
            var sql = ExpressionParser.ToSql<ClassC, ClassB>(
                (classA, classB) =>
                    classA.B_Ref != null && classA.B_Ref.HasValue && classA.B_Ref == classB.B_IntProperty, _keyMap);

            var expected = "((([c0].[B_Ref] <> '') AND ([c0].[B_Ref] <> '')) AND ([c0].[B_Ref] = [c1].[B_IntProperty]))";

            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
    }
}