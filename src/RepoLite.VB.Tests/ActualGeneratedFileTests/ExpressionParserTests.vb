Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RepoLite.VB.Tests.ActualGeneratedFileTests.Base
Imports RepoLite.VB.Tests.MODELNAMESPACE.Base
Imports RepoLite.VB.Tests.REPOSITORYNAMESPACE.Base


Namespace RepoLite.VB.Tests.ActualGeneratedFIlesTests
    Friend Class ClassA
        Implements IBaseModel

        Public ReadOnly Property EntityName As String Implements IBaseModel.EntityName
            Get
                Return "ClassA"
            End Get
        End Property

        Public Property A_StringProperty As String
        Public Property A_IntProperty As Integer
        Public Property A_BoolProperty As Boolean
        Public Property A_DateTimeProperty As DateTime
    End Class

    Friend Class ClassB
        Implements IBaseModel

        Public ReadOnly Property EntityName As String Implements IBaseModel.EntityName
            Get
                Return "ClassB"
            End Get
        End Property

        Public Property B_StringProperty As String
        Public Property B_IntProperty As Integer
        Public Property B_BoolProperty As Boolean
        Public Property B_DateTimeProperty As DateTime
    End Class

    <TestClass>
    Public Class ExpressionParserTests
        Inherits BaseTests

        <TestMethod>
        Public Sub TestExpressionParser_String_Equals_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(Function(classA, classB) classA.A_StringProperty = classB.B_StringProperty)
            Dim expected = "(([ClassA].[A_StringProperty] <> [ClassB].[B_StringProperty]) = 0)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotEquals_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_StringProperty <> classB.B_StringProperty)
            Dim expected = "(([ClassA].[A_StringProperty] <> [ClassB].[B_StringProperty]) <> 0)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_Contains_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_StringProperty.Contains(classB.B_StringProperty))
            Dim expected = "([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty]+'%')"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotContains_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) Not classA.A_StringProperty.Contains(classB.B_StringProperty))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty]+'%'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_StartsWith_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_StringProperty.StartsWith(classB.B_StringProperty))
            Dim expected = "([ClassA].[A_StringProperty] LIKE [ClassB].[B_StringProperty]+'%')"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotStartsWith_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) Not classA.A_StringProperty.StartsWith(classB.B_StringProperty))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] LIKE [ClassB].[B_StringProperty]+'%'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_EndsWith_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_StringProperty.EndsWith(classB.B_StringProperty))
            Dim expected = "([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotEndsWith_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) Not classA.A_StringProperty.EndsWith(classB.B_StringProperty))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%'+[ClassB].[B_StringProperty]))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_Equals_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_StringProperty = "hello")
            Dim expected = "(([ClassA].[A_StringProperty] <> 'hello') = 0)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotEquals_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_StringProperty <> "hello")
            Dim expected = "(([ClassA].[A_StringProperty] <> 'hello') <> 0)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_Contains_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_StringProperty.Contains("hello"))
            Dim expected = "([ClassA].[A_StringProperty] LIKE '%hello%')"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotContains_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) Not classA.A_StringProperty.Contains("hello"))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%hello%'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_StartsWith_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_StringProperty.StartsWith("hello"))
            Dim expected = "([ClassA].[A_StringProperty] LIKE 'hello%')"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotStartsWith_Val()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA)(Function(classA) Not classA.A_StringProperty.StartsWith("hello"))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] LIKE 'hello%'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_EndsWith_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_StringProperty.EndsWith("hello"))
            Dim expected = "([ClassA].[A_StringProperty] LIKE '%hello')"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotEndsWith_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) Not classA.A_StringProperty.EndsWith("hello"))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] LIKE '%hello'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_In_Val()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA)(
                        Function(classA) {"one", "two", "three"}.Contains(classA.A_StringProperty))
            Dim expected = "([ClassA].[A_StringProperty] IN ('one','two','three'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_String_NotIn_Val()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA)(
                        Function(classA) Not {"one", "two", "three"}.Contains(classA.A_StringProperty))
            Dim expected = "(NOT ([ClassA].[A_StringProperty] IN ('one','two','three')))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_Equals_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_IntProperty = classB.B_IntProperty)
            Dim expected = "([ClassA].[A_IntProperty] = [ClassB].[B_IntProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_NotEquals_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_IntProperty <> classB.B_IntProperty)
            Dim expected = "([ClassA].[A_IntProperty] <> [ClassB].[B_IntProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_LessThan_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_IntProperty < classB.B_IntProperty)
            Dim expected = "([ClassA].[A_IntProperty] < [ClassB].[B_IntProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_LessThanEqual_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_IntProperty <= classB.B_IntProperty)
            Dim expected = "([ClassA].[A_IntProperty] <= [ClassB].[B_IntProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_GreaterThan_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_IntProperty > classB.B_IntProperty)
            Dim expected = "([ClassA].[A_IntProperty] > [ClassB].[B_IntProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_GreaterThanEqual_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_IntProperty >= classB.B_IntProperty)
            Dim expected = "([ClassA].[A_IntProperty] >= [ClassB].[B_IntProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_Mod()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) (classA.A_IntProperty Mod classB.B_IntProperty) = 1)
            Dim expected = "(([ClassA].[A_IntProperty] % [ClassB].[B_IntProperty]) = 1)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_Mod_2()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) _
                                                                  (classA.A_IntProperty Mod classB.B_IntProperty) =
                                                                  (classB.B_IntProperty Mod classA.A_IntProperty))
            Dim expected =
                    "(([ClassA].[A_IntProperty] % [ClassB].[B_IntProperty]) = ([ClassB].[B_IntProperty] % [ClassA].[A_IntProperty]))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_Equals_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_IntProperty = 3)
            Dim expected = "([ClassA].[A_IntProperty] = 3)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_NotEquals_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_IntProperty <> 4)
            Dim expected = "([ClassA].[A_IntProperty] <> 4)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_LessThan_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_IntProperty < 5)
            Dim expected = "([ClassA].[A_IntProperty] < 5)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_LessThanEqual_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_IntProperty <= 5)
            Dim expected = "([ClassA].[A_IntProperty] <= 5)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_GreaterThan_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_IntProperty > 10)
            Dim expected = "([ClassA].[A_IntProperty] > 10)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_GreaterThanEqual_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_IntProperty >= 10)
            Dim expected = "([ClassA].[A_IntProperty] >= 10)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_In_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) {1, 2, 3}.Contains(classA.A_IntProperty))
            Dim expected = "([ClassA].[A_IntProperty] IN ('1','2','3'))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Int_NotIn_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) Not {1, 2, 3}.Contains(classA.A_IntProperty))
            Dim expected = "(NOT ([ClassA].[A_IntProperty] IN ('1','2','3')))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Bool_True_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_BoolProperty = classB.B_BoolProperty)
            Dim expected = "([ClassA].[A_BoolProperty] = [ClassB].[B_BoolProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Bool_True_Col_2()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) _
                                                                  classA.A_BoolProperty = classB.B_BoolProperty AndAlso
                                                                  Not classB.B_BoolProperty)
            Dim expected =
                    "(([ClassA].[A_BoolProperty] = [ClassB].[B_BoolProperty]) AND (NOT ([ClassB].[B_BoolProperty] = 1)))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Bool_False_Col()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_BoolProperty <> classB.B_BoolProperty)
            Dim expected = "([ClassA].[A_BoolProperty] <> [ClassB].[B_BoolProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Bool_True_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) classA.A_BoolProperty)
            Dim expected = "([ClassA].[A_BoolProperty] = 1)"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_Bool_False_Val()
            Dim sql = ExpressionParser.ToSql (Of ClassA)(Function(classA) Not classA.A_BoolProperty)
            Dim expected = "(NOT ([ClassA].[A_BoolProperty] = 1))"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_DateTime_LessThan()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_DateTimeProperty < classB.B_DateTimeProperty)
            Dim expected = "([ClassA].[A_DateTimeProperty] < [ClassB].[B_DateTimeProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub

        <TestMethod>
        Public Sub TestExpressionParser_DateTime_LessThanEqualTo()
            Dim sql =
                    ExpressionParser.ToSql (Of ClassA, ClassB)(
                        Function(classA, classB) classA.A_DateTimeProperty <= classB.B_DateTimeProperty)
            Dim expected = "([ClassA].[A_DateTimeProperty] <= [ClassB].[B_DateTimeProperty])"
            Assert.IsTrue(sql = expected, $"expected: {expected} but received: {sql}")
        End Sub
    End Class
End Namespace