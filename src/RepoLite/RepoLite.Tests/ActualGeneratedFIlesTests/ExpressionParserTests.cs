using System;
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
        #region [String] 
        
        [TestMethod]
        public void TestExpressionParser_String_Equals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_StringProperty == classB.B_StringProperty);

            var expected = "([ClassA].[A_StringProperty] = [ClassB].[B_StringProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotEquals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_StringProperty != classB.B_StringProperty);

            var expected = "([ClassA].[A_StringProperty] <> [ClassB].[B_StringProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_Contains_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_StringProperty.Contains(classB.B_StringProperty));

            var expected = "([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty]+'%')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotContains_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => !classA.A_StringProperty.Contains(classB.B_StringProperty));

            var expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty]+'%'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_StartsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_StringProperty.StartsWith(classB.B_StringProperty));

            var expected = "([ClassA].[A_StringProperty] LIKE [ClassB].[B_StringProperty]+'%')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotStartsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => !classA.A_StringProperty.StartsWith(classB.B_StringProperty));
            
            var expected = "(NOT ([ClassA].[A_StringProperty] LIKE [ClassB].[B_StringProperty]+'%'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_EndsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_StringProperty.EndsWith(classB.B_StringProperty));

            var expected = "([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotEndsWith_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => !classA.A_StringProperty.EndsWith(classB.B_StringProperty));
            
            var expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty]))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_Equals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty == "hello");

            var expected = "([ClassA].[A_StringProperty] = 'hello')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotEquals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty != "hello");

            var expected = "([ClassA].[A_StringProperty] <> 'hello')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_Contains_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty.Contains("hello"));

            var expected = "([ClassA].[A_StringProperty] LIKE '%hello%')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotContains_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_StringProperty.Contains("hello"));

            var expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%hello%'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_StartsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty.StartsWith("hello"));

            var expected = "([ClassA].[A_StringProperty] LIKE 'hello%')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotStartsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_StringProperty.StartsWith("hello"));
            
            var expected = "(NOT ([ClassA].[A_StringProperty] LIKE 'hello%'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_EndsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_StringProperty.EndsWith("hello"));

            var expected = "([ClassA].[A_StringProperty] LIKE '%hello')";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotEndsWith_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_StringProperty.EndsWith("hello"));
            
            var expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%hello'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_In_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => new []{"one", "two", "three"}.Contains(classA.A_StringProperty));
            
            var expected = "([ClassA].[A_StringProperty] IN ('one','two','three'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_String_NotIn_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !new []{"one", "two", "three"}.Contains(classA.A_StringProperty));
            
            var expected = "(NOT ([ClassA].[A_StringProperty] IN ('one','two','three')))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion

        #region [Int]
        
        [TestMethod]
        public void TestExpressionParser_Int_Equals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_IntProperty == classB.B_IntProperty);

            var expected = "([ClassA].[A_IntProperty] = [ClassB].[B_IntProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_NotEquals_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_IntProperty != classB.B_IntProperty);

            var expected = "([ClassA].[A_IntProperty] <> [ClassB].[B_IntProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_LessThan_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_IntProperty < classB.B_IntProperty);

            var expected = "([ClassA].[A_IntProperty] < [ClassB].[B_IntProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_LessThanEqual_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_IntProperty <= classB.B_IntProperty);

            var expected = "([ClassA].[A_IntProperty] <= [ClassB].[B_IntProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_GreaterThan_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_IntProperty > classB.B_IntProperty);

            var expected = "([ClassA].[A_IntProperty] > [ClassB].[B_IntProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_GreaterThanEqual_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_IntProperty >= classB.B_IntProperty);

            var expected = "([ClassA].[A_IntProperty] >= [ClassB].[B_IntProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_Mod()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => (classA.A_IntProperty % classB.B_IntProperty) == 1);

            var expected = "(([ClassA].[A_IntProperty] % [ClassB].[B_IntProperty]) = 1)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_Mod_2()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => (classA.A_IntProperty % classB.B_IntProperty) == (classB.B_IntProperty % classA.A_IntProperty));

            var expected = "(([ClassA].[A_IntProperty] % [ClassB].[B_IntProperty]) = ([ClassB].[B_IntProperty] % [ClassA].[A_IntProperty]))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_Equals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty == 3);

            var expected = "([ClassA].[A_IntProperty] = 3)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_NotEquals_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty != 4);

            var expected = "([ClassA].[A_IntProperty] <> 4)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_LessThan_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty < 5);

            var expected = "([ClassA].[A_IntProperty] < 5)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_LessThanEqual_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty <= 5);

            var expected = "([ClassA].[A_IntProperty] <= 5)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_GreaterThan_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty > 10);

            var expected = "([ClassA].[A_IntProperty] > 10)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_GreaterThanEqual_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_IntProperty >= 10);

            var expected = "([ClassA].[A_IntProperty] >= 10)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_In_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => new []{1,2,3}.Contains(classA.A_IntProperty));
            
            var expected = "([ClassA].[A_IntProperty] IN ('1','2','3'))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
        
        [TestMethod]
        public void TestExpressionParser_Int_NotIn_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !new []{1,2,3}.Contains(classA.A_IntProperty));
            
            var expected = "(NOT ([ClassA].[A_IntProperty] IN ('1','2','3')))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }


        #endregion

        #region [Bool]
       
        [TestMethod]
        public void TestExpressionParser_Bool_True_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_BoolProperty == classB.B_BoolProperty);
            
            var expected = "([ClassA].[A_BoolProperty] = [ClassB].[B_BoolProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
       
        [TestMethod]
        public void TestExpressionParser_Bool_True_Col_2()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_BoolProperty == classB.B_BoolProperty && !classB.B_BoolProperty);
            
            var expected = "(([ClassA].[A_BoolProperty] = [ClassB].[B_BoolProperty]) AND (NOT ([ClassB].[B_BoolProperty] = 1)))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
       
        [TestMethod]
        public void TestExpressionParser_Bool_False_Col()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_BoolProperty != classB.B_BoolProperty);
            
            var expected = "([ClassA].[A_BoolProperty] <> [ClassB].[B_BoolProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
       
        [TestMethod]
        public void TestExpressionParser_Bool_True_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => classA.A_BoolProperty);
            
            var expected = "([ClassA].[A_BoolProperty] = 1)";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }
       
        [TestMethod]
        public void TestExpressionParser_Bool_False_Val()
        {
            var sql = ExpressionParser.ToSql<ClassA>(classA => !classA.A_BoolProperty);
            
            var expected = "(NOT ([ClassA].[A_BoolProperty] = 1))";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion

        #region [DateTime]

        [TestMethod]
        public void TestExpressionParser_DateTime_LessThan()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_DateTimeProperty < classB.B_DateTimeProperty);
            
            var expected = "([ClassA].[A_DateTimeProperty] < [ClassB].[B_DateTimeProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        [TestMethod]
        public void TestExpressionParser_DateTime_LessThanEqualTo()
        {
            var sql = ExpressionParser.ToSql<ClassA, ClassB>((classA, classB) => classA.A_DateTimeProperty <= classB.B_DateTimeProperty);
            
            var expected = "([ClassA].[A_DateTimeProperty] <= [ClassB].[B_DateTimeProperty])";
            
            Assert.IsTrue(sql == expected, $"expected: {expected} but received: {sql}");
        }

        #endregion
    }
}