Imports System.Data.SqlClient
Imports System.Linq.Expressions
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Xml
Imports RepoLite.VB.Tests.MODELNAMESPACE.Base

Namespace REPOSITORYNAMESPACE.Base
    Public Interface IBaseRepository (Of T As {IBaseModel, New})
        Function GetAll() As IEnumerable(Of T)
        Function Create(item As T) As Boolean
        Function BulkCreate(items As List(Of T)) As Boolean
        Function BulkCreate(ParamArray items() As T) As Boolean

        Function Where(col As String, comparison As Comparison, val As Object) As Where(Of T)

        Function Where(col As String, comparison As Comparison, val As Object, valueType As Type) As Where(Of T)

        Function Where(query As String) As IEnumerable(Of T)
    End Interface

    Public Interface IPkRepository (Of T As {IBaseModel, New})
        Inherits IBaseRepository(Of T)
        Function Update(item As T) As Boolean
        Function Delete(item As T) As Boolean
        Function Delete(items As IEnumerable(Of T)) As Boolean
        Function Merge(items As List(Of T)) As Boolean
    End Interface

#Region "Enums"

    Friend Enum ClauseType
        [Initial]
        [And]
        [Or]
    End Enum

    Public Enum FindComparison
        [Equals]
        [NotEquals]
        [Like]
        [NotLike]
        [GreaterThan]
        [GreaterThanOrEquals]
        [LessThan]
        [LessThanOrEquals]
    End Enum

    Public Enum Comparison
        [Equals]
        [NotEquals]
        [Like]
        [NotLike]
        [GreaterThan]
        [GreaterThanOrEquals]
        [LessThan]
        [LessThanOrEquals]
        [In]
        [NotIn]
        [IsNull]
        [IsNotNull]
    End Enum

#End Region

    Public Class DeleteColumn
        Public Property ColumnName As String
        Public Property SqlDbType As SqlDbType
        Public Property Data As Object

        Public Sub New(columnName As String, data As Object, sqlDbType As SqlDbType)
            Me.ColumnName = columnName
            Me.Data = data
            Me.SqlDbType = sqlDbType
        End Sub
    End Class

    Public Class ColumnDefinition
        Public Property ColumnName As String
        Public Property ValueType As Type
        Public Property SqlDataTypeText As String
        Public Property SqlDbType As SqlDbType
        Public Property Identity As Boolean
        Public Property PrimaryKey As Boolean
        Public Property Nullable As Boolean


        Friend Sub New(columnName As String)
            Me.New(columnName, GetType(String), "NVARCHAR(MAX)", SqlDbType.NVarChar, False, False, False)
        End Sub

        Public Sub New(columnName As String, valueType As Type, sqlDataTypeText As String,
                       sqlDbType As SqlDbType)
            Me.New(columnName, valueType, sqlDataTypeText, sqlDbType, False, False, False)
        End Sub

        Public Sub New(columnName As String, valueType As Type, sqlDataTypeText As String,
                       sqlDbType As SqlDbType, nullable As Boolean)
            Me.New(columnName, valueType, sqlDataTypeText, sqlDbType, nullable, False, False)
        End Sub

        Public Sub New(columnName As String, valueType As Type, sqlDataTypeText As String,
                       sqlDbType As SqlDbType, nullable As Boolean, primaryKey As Boolean)
            Me.New(columnName, valueType, sqlDataTypeText, sqlDbType, nullable, primaryKey, False)
        End Sub

        Public Sub New(columnName As String, valueType As Type, sqlDataTypeText As String,
                       sqlDbType As SqlDbType, nullable As Boolean, primaryKey As Boolean,
                       identity As Boolean)
            Me.ColumnName = columnName
            Me.ValueType = valueType
            Me.SqlDataTypeText = sqlDataTypeText
            Me.SqlDbType = sqlDbType
            Me.Nullable = nullable
            Me.PrimaryKey = primaryKey
            Me.Identity = identity
        End Sub
    End Class

    Public Class QueryItem
        Public Property DbColumnName As String
        Public Property Value As Object
        Public Property DataType As Type

        Public Sub New(dbColName As String, value As Object)
            Me.New(dbColName, value, value.[GetType]())
        End Sub

        Public Sub New(dbColumnName As String, value As Object, dataType As Type)
            Me.DbColumnName = dbColumnName
            Me.Value = value
            Me.DataType = dataType
        End Sub
    End Class

    Public Class ValidationException
        Inherits Exception

        Public Property ValidationErrors As List(Of ValidationError)

        Public Sub New(validationErrors As List(Of ValidationError))
            Me.ValidationErrors = validationErrors
        End Sub
    End Class

    Public Class Where (Of T As {IBaseModel, New})
        Private ReadOnly _query As StringBuilder = New StringBuilder()
        Private ReadOnly _repository As BaseRepository(Of T)
        Private _activeGroups As Integer

        Public Sub New(ByVal baseRepository As BaseRepository(Of T), ByVal col As String, ByVal comparison As Comparison,
                       ByVal val As Object)
            Me.New(baseRepository, col, comparison, val, val.[GetType]())
        End Sub

        Public Sub New(ByVal baseRepository As BaseRepository(Of T), ByVal col As String, ByVal comparison As Comparison,
                       ByVal val As Object, ByVal valueType As Type)
            _repository = baseRepository
            _query.Append(MakeClause(col, comparison, val, ClauseType.Initial, valueType))
        End Sub

        Private Function MakeClause(ByVal col As String, ByVal comparison As Comparison, ByVal clauseType As ClauseType) _
            As String
            Return MakeClause(col, comparison, Nothing, clauseType, Nothing)
        End Function


        Private Function MakeClause(ByVal col As String, ByVal comparison As Comparison, ByVal val As Object,
                                    ByVal clauseType As ClauseType, ByVal valueType As Type) As String
            Dim query = New StringBuilder()

            Select Case comparison
                Case Comparison.[In], Comparison.NotIn
                    If _
                        TryCast(val, IEnumerable) IsNot Nothing And
                        Not If(TryCast(val, Object()), TryCast(val, IEnumerable).Cast (Of Object)().ToArray()).Any() _
                        Then
                        query.Append("1=0")
                        Return query.ToString()
                    End If
            End Select

            Dim floatVal As Single
            If {Comparison.GreaterThan, Comparison.GreaterThanOrEquals, Comparison.LessThan,
                Comparison.LessThanOrEquals}.Contains(comparison) AndAlso Not Single.TryParse(val.ToString(), floatVal) _
                Then
                Throw New Exception("Numeric comparison used on a non numeric value.")
            End If

            Select Case clauseType
                Case ClauseType.Initial
                    query.Append(
                        If(valueType = GetType(XmlDocument), "CONVERT(NVARCHAR(MAX), [" & col & "])", "[" & col & "]"))
                Case ClauseType.[And]
                    query.Append(
                        If (valueType = GetType(XmlDocument), " AND CONVERT(NVARCHAR(MAX), [" & col & "])",
                            " AND [" & col & "]"))
                Case ClauseType.[Or]
                    query.Append(
                        If (valueType = GetType(XmlDocument), " OR CONVERT(NVARCHAR(MAX), [" & col & "])",
                            " OR [" & col & "]"))
            End Select

            query.Append(GetComparison(comparison))

            If comparison <> Comparison.IsNull AndAlso comparison <> Comparison.IsNotNull Then
                Dim typeVal = GetTypeVal(col, val)
                If comparison = Comparison.[Like] OrElse comparison = Comparison.NotLike Then _
                    typeVal = typeVal.TrimStart("'"c).TrimEnd("'"c)
                query.Append(typeVal)
            End If

            Select Case comparison
                Case Comparison.[In], Comparison.NotIn
                    query.Append(")")
                Case Comparison.[Like], Comparison.NotLike
                    query.Append("%'")
            End Select

            Return query.ToString()
        End Function

        Private Shared Function GetComparison(ByVal comparison As Comparison) As String
            Select Case comparison
                Case Comparison.Equals
                    Return " = "
                Case Comparison.NotEquals
                    Return " <> "
                Case Comparison.[Like]
                    Return " LIKE '%"
                Case Comparison.NotLike
                    Return " NOT LIKE '%"
                Case Comparison.GreaterThan
                    Return " > "
                Case Comparison.GreaterThanOrEquals
                    Return " >= "
                Case Comparison.LessThan
                    Return " < "
                Case Comparison.LessThanOrEquals
                    Return " <= "
                Case Comparison.[In]
                    Return " IN ("
                Case Comparison.NotIn
                    Return " NOT IN ("
                Case Comparison.IsNull
                    Return " IS NULL"
                Case Comparison.IsNotNull
                    Return " IS NOT NULL"
                Case Else
                    Throw New NotSupportedException("???")
            End Select
        End Function

        Private Function GetTypeVal(ByVal col As String, ByVal val As Object) As String
            Dim typeName = If(TypeOf val Is IList, "List", val.[GetType]().Name)

            Select Case typeName
                Case "Boolean"
                    If CBool(val) Then Return "1"
                    Return "0"
                Case "Int16", "UInt16", "Int32", "UInt32", "Int64", "UInt64", "Decimal", "Double"
                    Return val.ToString()
                Case "DateTime", "Char", "String", "Guid", "TimeSpan", "DateTimeOffset"
                    Return "'" & val & "'"
                Case "List"
                    Dim result = ""
                    Dim enumerable =
                            If(TryCast(val, Object()), (TryCast(val, IEnumerable)).Cast (Of Object)().ToArray())
                    Const batchSize As Integer = 2000
                    Dim batches = Math.Ceiling(CDec(enumerable.Length)/batchSize)

                    For i = 0 To batches - 1
                        result =
                            enumerable.Skip(i*batchSize).Take(batchSize).Aggregate(result,
                                                                                   Function(current, o) _
                                                                                      current & GetTypeVal(col, o) &
                                                                                      ", ").TrimEnd(" "c).TrimEnd(","c)
                        If batches > i + 1 Then result += ") OR [" & col & "] IN ("
                    Next

                    Return result
                Case Else
                    Throw New NotSupportedException("Not supported yet")
            End Select
        End Function

        Public Function Results() As IEnumerable(Of T)
            If _activeGroups > 0 Then Throw New Exception("Please close all Query Groups before calling Results()")
            Return _repository.Where(_query.ToString())
        End Function

        Public Function [And](ByVal col As String, ByVal comparison As Comparison) As Where(Of T)
            If comparison <> Comparison.IsNull AndAlso comparison <> Comparison.IsNotNull Then _
                Throw _
                    New Exception(
                        "And(" & col & ", " & comparison &
                        ") can only be called with Comparison.IsNull or Comparison.IsNotNull")
            _query.Append(MakeClause(col, comparison, ClauseType.[And]))
            Return Me
        End Function

        Public Function [And](ByVal col As String, ByVal comparison As Comparison, ByVal val As Object) As Where(Of T)
            Return [And](col, comparison, val, val.[GetType]())
        End Function

        Public Function [And](ByVal col As String, ByVal comparison As Comparison, ByVal val As Object,
                              ByVal valueType As Type) As Where(Of T)
            _query.Append(MakeClause(col, comparison, val, ClauseType.[And], valueType))
            Return Me
        End Function

        Public Function [Or](ByVal col As String, ByVal comparison As Comparison) As Where(Of T)
            If comparison <> Comparison.IsNull AndAlso comparison <> Comparison.IsNotNull Then _
                Throw _
                    New Exception(
                        "Or(" & col & ", " & comparison &
                        ") can only be called with Comparison.IsNull or Comparison.IsNotNull")
            _query.Append(MakeClause(col, comparison, ClauseType.[Or]))
            Return Me
        End Function

        Public Function [Or](ByVal col As String, ByVal comparison As Comparison, ByVal val As Object) As Where(Of T)
            Return [Or](col, comparison, val, val.[GetType]())
        End Function

        Public Function [Or](ByVal col As String, ByVal comparison As Comparison, ByVal val As Object,
                             ByVal valueType As Type) As Where(Of T)
            _query.Append(MakeClause(col, comparison, val, ClauseType.[Or], valueType))
            Return Me
        End Function

        Public Function AndBeginGroup(ByVal col As String, ByVal comparison As Comparison) As Where(Of T)
            If comparison <> Comparison.IsNull AndAlso comparison <> Comparison.IsNotNull Then _
                Throw _
                    New Exception(
                        "AndBeginGroup(" & col & ", " & comparison &
                        ") can only be called with Comparison.IsNull or Comparison.IsNotNull")
            _activeGroups += 1
            _query.Append(" AND (" & MakeClause(col, comparison, ClauseType.Initial))
            Return Me
        End Function

        Public Function AndBeginGroup(ByVal col As String, ByVal comparison As Comparison, ByVal val As Object) _
            As Where(Of T)
            Return AndBeginGroup(col, comparison, val, val.[GetType]())
        End Function

        Public Function AndBeginGroup(ByVal col As String, ByVal comparison As Comparison, ByVal val As Object,
                                      ByVal valueType As Type) As Where(Of T)
            _activeGroups += 1
            _query.Append(" AND (" & MakeClause(col, comparison, val, ClauseType.Initial, valueType))
            Return Me
        End Function

        Public Function OrBeginGroup(ByVal col As String, ByVal comparison As Comparison) As Where(Of T)
            If comparison <> Comparison.IsNull AndAlso comparison <> Comparison.IsNotNull Then _
                Throw _
                    New Exception(
                        "OrBeginGroup(" & col & ", " & comparison &
                        ") can only be called with Comparison.IsNull or Comparison.IsNotNull")
            _activeGroups += 1
            _query.Append(" OR (" & MakeClause(col, comparison, ClauseType.Initial))
            Return Me
        End Function

        Public Function OrBeginGroup(ByVal col As String, ByVal comparison As Comparison, ByVal val As Object) _
            As Where(Of T)
            Return OrBeginGroup(col, comparison, val, val.[GetType]())
        End Function

        Public Function OrBeginGroup(ByVal col As String, ByVal comparison As Comparison, ByVal val As Object,
                                     ByVal valueType As Type) As Where(Of T)
            _activeGroups += 1
            _query.Append(" OR (" & MakeClause(col, comparison, val, ClauseType.Initial, valueType))
            Return Me
        End Function

        Public Function EndGroup() As Where(Of T)
            _activeGroups -= 1
            _query.Append(")")
            Return Me
        End Function

        Public Function QueryString() As String
            Return _repository.WhereQuery() & " WHERE " & _query.ToString()
        End Function
    End Class

    Friend Class ExpressionParser
        Private Shared ReadOnly _
            NodeStr As Dictionary(Of ExpressionType, String) = New Dictionary(Of ExpressionType, String) From {
            {ExpressionType.Add, "+"},
            {ExpressionType.[And], "&"},
            {ExpressionType.[AndAlso], "AND"},
            {ExpressionType.Convert, ""},
            {ExpressionType.Divide, "/"},
            {ExpressionType.Equal, "="},
            {ExpressionType.ExclusiveOr, "^"},
            {ExpressionType.GreaterThan, ">"},
            {ExpressionType.GreaterThanOrEqual, ">="},
            {ExpressionType.LessThan, "<"},
            {ExpressionType.LessThanOrEqual, "<="},
            {ExpressionType.Modulo, "%"},
            {ExpressionType.Multiply, "*"},
            {ExpressionType.Negate, "-"},
            {ExpressionType.[Not], "NOT"},
            {ExpressionType.NotEqual, "<>"},
            {ExpressionType.[Or], "|"},
            {ExpressionType.[OrElse], "OR"},
            {ExpressionType.Subtract, "-"}
            }

        Friend Shared Function ToSql(expression As LambdaExpression, xRef As Dictionary(Of String, String)) As String
            Return Parse(expression.Body, xRef, True).Sql
        End Function

        Friend Shared Function ToSql (Of T As IBaseModel)(ByVal expression As Expression(Of Func(Of T, Boolean)),
                                                          xRef As Dictionary(Of String, String)) _
            As String
            Return ToSql(CType(expression, LambdaExpression), xRef)
        End Function

        Friend Shared Function ToSql (Of T As IBaseModel, TK As IBaseModel)(
                                                                            ByVal expression As _
                                                                               Expression(Of Func(Of T, TK, Boolean)),
                                                                            xRef As Dictionary(Of String, String)) _
            As String
            Return ToSql(CType(expression, LambdaExpression), xRef)
        End Function

        Shared Private Function Parse(ByVal expression As Expression, xRef As Dictionary(Of String, String),
                                      ByVal Optional isUnary As Boolean = False,
                                      ByVal Optional prefix As String = Nothing,
                                      ByVal Optional postfix As String = Nothing,
                                      ByVal Optional boolComparison As Boolean = False) As Clause

            While True

                If TypeOf expression Is UnaryExpression Then
                    Dim unary = TryCast(expression, UnaryExpression)
                    Return Clause.Make(NodeStr(unary.NodeType), Parse(unary.Operand, xRef, True))
                ElseIf TypeOf expression Is BinaryExpression Then
                    Dim body = TryCast(expression, BinaryExpression)
                    Dim left As Clause
                    If (body.Left.Type = GetType(Boolean))
                        left = Parse(body.Left, xRef, boolComparison := True)
                    Else
                        left = Parse(body.Left, xRef)
                    End If

                    Dim right =
                            If _
                            (body.Right.Type = GetType(Boolean), Parse(body.Right, xRef, boolComparison := True),
                             Parse(body.Right, xRef))
                    Return Clause.Make(left, NodeStr(body.NodeType), right)
                ElseIf TypeOf expression Is ConstantExpression Then
                    Dim constant = TryCast(expression, ConstantExpression)
                    Dim value = constant.Value

                    If TypeOf value Is Integer Then
                        Return Clause.Make(value.ToString())
                    ElseIf TypeOf value Is String Then
                        value = prefix & CStr(value) + postfix
                    End If

                    If TypeOf value Is Boolean AndAlso isUnary Then
                        Return _
                            If _
                                (boolComparison, Clause.Make($"'{value}'"),
                                 Clause.Make(Clause.Make($"'{value}'"), "=", Clause.Make("1")))
                    End If

                    Return Clause.Make($"'{value}'")
                ElseIf TypeOf expression Is MemberExpression Then
                    Dim member = TryCast(expression, MemberExpression)

                    If TypeOf member.Member Is PropertyInfo Then
                        Dim [property] = TryCast(member.Member, PropertyInfo)

                        If [property].Name = "Value" And [property].DeclaringType IsNot Nothing _
                           AndAlso Nullable.GetUnderlyingType([property].DeclaringType) IsNot Nothing Then
                            Return Parse(member.Expression, xRef)
                        End If

                        If [property].Name = "HasValue" And [property].DeclaringType IsNot Nothing _
                           AndAlso Nullable.GetUnderlyingType([property].DeclaringType) IsNot Nothing Then
                            Return _
                                Clause.Make(Parse(member.Expression, xRef), NodeStr(ExpressionType.NotEqual),
                                            Clause.Make("''"))
                        End If


                        Dim colName = [property].Name

                        Dim [alias] = ""
                        If TypeOf member.Expression Is ParameterExpression Then
                            Dim paramExpr = TryCast(member.Expression, ParameterExpression)
                            If xRef.ContainsKey(paramExpr.Name) Then [alias] = xRef(paramExpr.Name)
                        End If

                        If member.Type = GetType(Boolean) Then

                            If isUnary Then
                                isUnary = False
                                prefix = Nothing
                                postfix = Nothing
                                Continue While
                            Else
                                Return If (boolComparison,
                                           Clause.Make(
                                               If (Not String.IsNullOrEmpty([alias]), $"[{[alias]}].[{colName}]",
                                                   $"[{colName}]")),
                                           Clause.Make(
                                               Clause.Make(
                                                   If (Not String.IsNullOrEmpty([alias]),
                                                       $"[{[alias]}].[{colName}]", $"[{colName}]")), "=",
                                               Clause.Make("1")))
                            End If
                        Else
                            Dim value =
                                    If(Not String.IsNullOrEmpty([alias]), $"[{[alias]}].[{colName}]", $"[{colName}]")
                            If Not String.IsNullOrEmpty(prefix) Then value = $"'{prefix}'+{value}"
                            If Not String.IsNullOrEmpty(postfix) Then value = $"{value}+'{postfix}'"
                            Return Clause.Make(value)
                        End If
                    ElseIf TypeOf member.Member Is FieldInfo Then
                        Dim value = GetValue(member)

                        If TypeOf value Is String Then
                            value = prefix & CStr(value) & postfix
                        End If

                        Return Clause.Make($"'{value}'")
                    Else
                        Throw New Exception($"Expression does not refer to a property or field: {expression}")
                    End If
                ElseIf TypeOf expression Is MethodCallExpression Then
                    Dim methodCall = TryCast(expression, MethodCallExpression)

                    If methodCall.Method = GetType(String).GetMethod("Contains", {GetType(String)}) Then
                        Return Clause.Make(Parse(methodCall.Object, xRef), "LIKE",
                                        Parse(methodCall.Arguments(0), xRef, prefix := "%", postfix := "%"))
                    End If

                    If methodCall.Method = GetType(String).GetMethod("StartsWith", {GetType(String)}) Then
                        Return Clause.Make(Parse(methodCall.Object, xRef), "LIKE",
                                        Parse(methodCall.Arguments(0), xRef, postfix := "%"))
                    End If

                    If methodCall.Method = GetType(String).GetMethod("EndsWith", {GetType(String)}) Then
                        Return Clause.Make(Parse(methodCall.Object, xRef), "LIKE",
                                        Parse(methodCall.Arguments(0), xRef, prefix := "%"))
                    End If

                    If methodCall.Method.Name = "Contains" Then
                        Dim collection As Expression
                        Dim [property] As Expression

                        If methodCall.Method.IsDefined(GetType(ExtensionAttribute)) AndAlso
                            methodCall.Arguments.Count = 2 Then
                            collection = methodCall.Arguments(0)
                            [property] = methodCall.Arguments(1)
                        ElseIf Not methodCall.Method.IsDefined(GetType(ExtensionAttribute)) AndAlso
                            methodCall.Arguments.Count = 1 Then
                            collection = methodCall.Object
                            [property] = methodCall.Arguments(0)
                        Else
                            Throw New Exception("Unsupported method call: " & methodCall.Method.Name)
                        End If

                        Dim sb = New StringBuilder()

                        For Each iVal In CType(GetValue(collection), IEnumerable)
                            sb.Append($"'{iVal}',")
                        Next

                        Dim values = sb.ToString()
                        values = values.Substring(0, values.Length - 1)
                        Return Clause.Make(Parse([property], xRef), "IN", Clause.Make($"({values})"))
                    End If

                    if (methodCall.Method.Name = "CompareString")
                        If methodCall.Arguments.Count >= 2 Then
                            If TypeOf methodCall.Arguments(2) is ConstantExpression
                                Dim constantExpression = TryCast(methodCall.Arguments(2), ConstantExpression)
                                Return Clause.Make(Parse(methodCall.Arguments(0), xRef),
                                                If(constantExpression.Value.ToString().ToLower() = "false", "<>", "="),
                                                Parse(methodCall.Arguments(1), xRef))
                            End If
                        End If
                    End If

                    Throw New Exception("Unsupported method call: " & methodCall.Method.Name)
                Else
                    Throw New Exception("Unsupported expression: " & expression.[GetType]().Name)
                End If
            End While
        End Function

        Shared Private Function GetValue(ByVal member As Expression) As Object
            Dim objectMember = Expression.Convert(member, GetType(Object))
            Dim getterLambda = Expression.Lambda (Of Func(Of Object))(objectMember)
            Dim getter = getterLambda.Compile()
            Return getter()
        End Function

        Private Class Clause
            Public Property Sql As String

            Public Shared Function Make(ByVal sql As String) As Clause
                Return New Clause With {
                    .Sql = sql
                    }
            End Function

            Public Shared Function Make(ByVal [operator] As String, ByVal operand As Clause) As Clause
                Return New Clause With {
                    .Sql = If(String.IsNullOrEmpty([operator]), $"({[operator]} {operand.Sql})", $"{operand.Sql}")
                    }
            End Function

            Public Shared Function Make(ByVal left As Clause, ByVal [operator] As String, ByVal right As Clause) _
                As Clause
                Return New Clause With {
                    .Sql = $"({left.Sql} {[operator]} {right.Sql})"
                    }
            End Function
        End Class
    End Class

#region "OnClause"

    Public Interface IOnClause (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New})
        Function [On](ByVal expr As Expression(Of Func(Of T1, T2, Boolean))) As ICombinedRepository(Of T1, T2)
    End Interface

    Public Interface IOnClause (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New})
        Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, Boolean))) As ICombinedRepository(Of T1, T2, T3)
    End Interface

    Public Interface IOnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New})

        Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4)
    End Interface

    Public Interface IOnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New})

        Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, T5, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4, T5)
    End Interface

    Public Interface IOnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New})

        Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, T5, T6, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4, T5, T6)
    End Interface

    Public Interface IOnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New}, T7 As {IBaseModel, New})

        Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, T5, T6, T7, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4, T5, T6, T7)
    End Interface

    Public MustInherit Class BaseOnClause
        Protected Friend ConnectionString As String
        Friend ReadOnly Property JoinInfo As JoinInfo

        Protected Sub New()
            Me.New(New JoinInfo())
        End Sub

        Protected Sub New(ByVal previousJoinInfo As JoinInfo)
            JoinInfo = previousJoinInfo
        End Sub

        Protected Sub AddJoin(ByVal type As Type, ByVal joinType As JoinType)
            If String.IsNullOrEmpty(type.FullName) Then Throw New Exception("Unsupported Type")
            Dim lastJoinToTable = JoinInfo.Joins.LastOrDefault(Function(x) x.ModelType = type)
            Dim [alias] = $"c{JoinInfo.Joins.Count + 1}"
            JoinInfo.Joins.Add(New Join(type, joinType, [alias]))
        End Sub

        Protected Sub AddExpression(ByVal type As Type, ByVal expr As LambdaExpression)
            If String.IsNullOrEmpty(type.FullName) Then Throw New Exception("Unsupported Type")
            Dim paramXRef = New Dictionary(Of String, String)()
            Dim i = 0

            For Each parameter In expr.Parameters
                paramXRef.Add(parameter.Name, $"c{Math.Min(System.Threading.Interlocked.Increment(i), i - 1)}")
            Next

            JoinInfo.Joins.Last().Expression = expr
            JoinInfo.Joins.Last().XRef = paramXRef
        End Sub
    End Class

    Public Class OnClause (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New})
        Inherits BaseOnClause
        Implements IOnClause(Of T1, T2)

        Public Sub New(ByVal connectionString As String, ByVal joinType As JoinType)
            ConnectionString = connectionString
            JoinInfo.InitialType = GetType(T1)
            AddJoin(GetType(T2), joinType)
        End Sub

        Public Function [On](ByVal expr As Expression(Of Func(Of T1, T2, Boolean))) As ICombinedRepository(Of T1, T2) _
            Implements IOnClause(Of T1, T2).On
            AddExpression(GetType(T2), expr)
            Return New CombinedRepository(Of T1, T2)(Me)
        End Function
    End Class

    Public Class OnClause (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New})
        Inherits BaseOnClause
        Implements IOnClause(Of T1, T2, T3)

        Public Sub New(ByVal previousJoinInfo As JoinInfo, ByVal joinType As JoinType)
            MyBase.New(previousJoinInfo)
            AddJoin(GetType(T3), joinType)
        End Sub

        Public Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3) Implements IOnClause(Of T1, T2, T3).On
            AddExpression(GetType(T3), expr)
            Return New CombinedRepository(Of T1, T2, T3)(Me)
        End Function
    End Class

    Public Class OnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New})
        Inherits BaseOnClause
        Implements IOnClause(Of T1, T2, T3, T4)

        Public Sub New(ByVal previousJoinInfo As JoinInfo, ByVal joinType As JoinType)
            MyBase.New(previousJoinInfo)
            AddJoin(GetType(T4), joinType)
        End Sub

        Public Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4) Implements IOnClause(Of T1, T2, T3, T4).On
            AddExpression(GetType(T4), expr)
            Return New CombinedRepository(Of T1, T2, T3, T4)(Me)
        End Function
    End Class

    Public Class OnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New})
        Inherits BaseOnClause
        Implements IOnClause(Of T1, T2, T3, T4, T5)

        Public Sub New(ByVal previousJoinInfo As JoinInfo, ByVal joinType As JoinType)
            MyBase.New(previousJoinInfo)
            AddJoin(GetType(T5), joinType)
        End Sub

        Public Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, T5, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4, T5) Implements IOnClause(Of T1, T2, T3, T4, T5).On
            AddExpression(GetType(T5), expr)
            Return New CombinedRepository(Of T1, T2, T3, T4, T5)(Me)
        End Function
    End Class

    Public Class OnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New})
        Inherits BaseOnClause
        Implements IOnClause(Of T1, T2, T3, T4, T5, T6)

        Public Sub New(ByVal previousJoinInfo As JoinInfo, ByVal joinType As JoinType)
            MyBase.New(previousJoinInfo)
            AddJoin(GetType(T6), joinType)
        End Sub

        Public Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, T5, T6, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4, T5, T6) Implements IOnClause(Of T1, T2, T3, T4, T5, T6).On
            AddExpression(GetType(T6), expr)
            Return New CombinedRepository(Of T1, T2, T3, T4, T5, T6)(Me)
        End Function
    End Class

    Public Class OnClause _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New}, T7 As {IBaseModel, New})
        Inherits BaseOnClause
        Implements IOnClause(Of T1, T2, T3, T4, T5, T6, T7)

        Public Sub New(ByVal previousJoinInfo As JoinInfo, ByVal joinType As JoinType)
            MyBase.New(previousJoinInfo)
            AddJoin(GetType(T7), joinType)
        End Sub

        Public Function [On](ByVal expr As Expression(Of Func(Of T1, T2, T3, T4, T5, T6, T7, Boolean))) _
            As ICombinedRepository(Of T1, T2, T3, T4, T5, T6, T7) Implements IOnClause(Of T1, T2, T3, T4, T5, T6, T7).On
            AddExpression(GetType(T7), expr)
            Return New CombinedRepository(Of T1, T2, T3, T4, T5, T6, T7)(Me)
        End Function
    End Class

#End Region

#region "Join Info"

    Public Enum JoinType
        Inner
        Left
        Right
        Full
    End Enum

    Public Class Join
        Public ReadOnly Property ModelType As Type
        Public ReadOnly Property JoinType As JoinType
        Public Property Expression As LambdaExpression
        Public Property [Alias] As String
        Public Property XRef As Dictionary(Of String, String)

        Public Sub New(ByVal type As Type, ByVal join As JoinType, ByVal [alias] As String)
            ModelType = type
            JoinType = join
            [Alias] = [alias]
        End Sub

        Public Function JoinString() As String
            Select Case JoinType
                Case JoinType.Inner
                    Return "INNER JOIN "
                Case JoinType.Left
                    Return "LEFT OUTER JOIN "
                Case JoinType.Right
                    Return "RIGHT OUTER JOIN "
                Case JoinType.Full
                    Return "FULL OUTER JOIN "
                Case Else
                    Throw New ArgumentOutOfRangeException()
            End Select
        End Function
    End Class

    Public Class JoinInfo
        Public Property InitialType As Type
        Public ReadOnly Property Joins As List(Of Join) = New List(Of Join)()
    End Class

    Public Interface IJoinable (Of T1 As {IBaseModel, New})
        Function InnerJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T1, T2)
        Function LeftJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T1, T2)
        Function RightJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T1, T2)
        Function FullJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T1, T2)
    End Interface

    Public Interface IJoinable (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New})
        Function InnerJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3)
        Function LeftJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3)
        Function RightJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3)
        Function FullJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3)
    End Interface

    Public Interface IJoinable (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New})
        Function InnerJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4)
        Function LeftJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4)
        Function RightJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4)
        Function FullJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4)
    End Interface

    Public Interface IJoinable _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New})
        Function InnerJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5)
        Function LeftJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5)
        Function RightJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5)
        Function FullJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5)
    End Interface

    Public Interface IJoinable _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New})
        Function InnerJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6)
        Function LeftJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6)
        Function RightJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6)
        Function FullJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6)
    End Interface

    Public Interface IJoinable _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New})
        Function InnerJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7)
        Function LeftJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7)
        Function RightJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7)
        Function FullJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7)
    End Interface

#end region

#region "Combined Repository"

    Public Interface ICombinedRepository
        ReadOnly Property JoinInfo() As JoinInfo
    End Interface

    Public Interface ICombinedRepository (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New})
        Inherits IJoinable(Of T1, T2), ICombinedRepository
        Function Results() As IEnumerable(Of Tuple(Of T1, T2))
    End Interface

    Public Interface ICombinedRepository (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New})
        Inherits IJoinable(Of T1, T2, T3), ICombinedRepository
        Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3))
    End Interface


    Public Interface ICombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New})
        Inherits IJoinable(Of T1, T2, T3, T4), ICombinedRepository
        Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4))
    End Interface

    Public Interface ICombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New})
        Inherits IJoinable(Of T1, T2, T3, T4, T5), ICombinedRepository
        Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4, T5))
    End Interface

    Public Interface ICombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New})
        Inherits IJoinable(Of T1, T2, T3, T4, T5, T6), ICombinedRepository
        Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4, T5, T6))
    End Interface

    Public Interface ICombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New}, T7 As {IBaseModel, New})
        Inherits ICombinedRepository
        Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4, T5, T6, T7))
    End Interface

    Public MustInherit Class CombinedRepositoryBase
        Inherits RepositoryDataAccess
        Implements ICombinedRepository

        Protected Const SqlParamSeparator As String = "__"
        Public ReadOnly Property JoinInfo As JoinInfo Implements ICombinedRepository.JoinInfo

        Protected Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause.ConnectionString)
            JoinInfo = previousOnClause.JoinInfo
        End Sub

        Private Function GenerateQuery() As String
            Dim sql = $"SELECT "
            If Not (TypeOf Activator.CreateInstance(JoinInfo.InitialType) Is IBaseModel) Then _
                Throw New Exception($"Initial Type wasn't an IBaseModel, but was {JoinInfo.InitialType}")
            Dim iInst = TryCast(Activator.CreateInstance(JoinInfo.InitialType), IBaseModel)
            Dim prop = JoinInfo.InitialType.GetProperty("Columns", BindingFlags.[Static])
            If prop Is Nothing Then _
                Throw _
                    New Exception(
                        $"internal static Columns property could not be found on {JoinInfo.InitialType.FullName}")
            If Not (TypeOf prop.GetValue(iInst) Is List(Of ColumnDefinition)) Then _
                Throw _
                    New Exception(
                        $"internal static Columns property is not of type List<ColumnDefinition> on { _
                                     JoinInfo.InitialType.FullName}")
            Dim iCols = TryCast(prop.GetValue(iInst), List(Of ColumnDefinition))

            For Each column In iCols
                sql += $"[c0].[{column.ColumnName}] AS '{iInst.EntityName}{SqlParamSeparator}{column.ColumnName}', "
            Next

            For Each join In JoinInfo.Joins
                If Not (TypeOf Activator.CreateInstance(join.ModelType) Is IBaseModel) Then _
                    Throw New Exception($"Join Type wasn't an IBaseModel, but was {join.ModelType}")
                Dim tInst = TryCast(Activator.CreateInstance(join.ModelType), IBaseModel)
                prop = join.ModelType.GetProperty("Columns", BindingFlags.[Static])
                If prop Is Nothing Then _
                    Throw _
                        New Exception(
                            $"internal static Columns property could not be found on {join.ModelType.FullName}")
                If Not (TypeOf prop.GetValue(tInst) Is List(Of ColumnDefinition)) Then _
                    Throw _
                        New Exception(
                            $"internal static Columns property is not of type List<ColumnDefinition> on { _
                                         join.ModelType.FullName}")
                Dim tCols = TryCast(prop.GetValue(tInst), List(Of ColumnDefinition))

                For Each column In tCols
                    sql +=
                        $"[{join.[Alias]}].[{column.ColumnName}] AS '{tInst.EntityName}{SqlParamSeparator}{ _
                            column.ColumnName}', "
                Next
            Next

            sql = sql.Substring(0, sql.Length - 2)
            sql += $" FROM [{iInst.EntityName}] c0 "

            For Each join In JoinInfo.Joins
                If Not (TypeOf Activator.CreateInstance(join.ModelType) Is IBaseModel) Then _
                    Throw New Exception($"Join Type wasn't an IBaseModel, but was {join.ModelType}")
                Dim tInst = TryCast(Activator.CreateInstance(join.ModelType), IBaseModel)
                Dim expressionSql = ExpressionParser.ToSql(join.Expression, join.XRef)
                sql += $" {join.JoinString()} [{tInst.EntityName}] {join.[Alias]} ON {expressionSql}"
            Next

            Return sql
        End Function

        Protected Function BaseResults() As DataTable
            Dim query = GenerateQuery()
            Dim dt As DataTable

            Using cn = New SqlConnection(ConnectionString)

                Using cmd = CreateCommand(cn, query)
                    If HasInjection(cmd.CommandText) Then Throw New Exception("Sql Injection attempted. Aborted")
                    cn.Open()
                    dt = ToDataTable(cmd)
                    If dt Is Nothing Then Return Nothing
                    cn.Close()
                End Using
            End Using

            Return dt
        End Function
    End Class

    Public Class CombinedRepository (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New})
        Inherits CombinedRepositoryBase
        Implements ICombinedRepository(Of T1, T2)

        Public Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause)
        End Sub

        Public Function InnerJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3) _
            Implements ICombinedRepository(Of T1, T2).InnerJoin
            Return Join (Of T3)(JoinType.Inner)
        End Function

        Public Function LeftJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3) _
            Implements ICombinedRepository(Of T1, T2).LeftJoin
            Return Join (Of T3)(JoinType.Left)
        End Function

        Public Function RightJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3) _
            Implements ICombinedRepository(Of T1, T2).RightJoin
            Return Join (Of T3)(JoinType.Right)
        End Function

        Public Function FullJoin (Of T3 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3) _
            Implements ICombinedRepository(Of T1, T2).FullJoin
            Return Join (Of T3)(JoinType.Full)
        End Function

        Private Function Join (Of T3 As {IBaseModel, New})(ByVal joinType As JoinType) As IOnClause(Of T1, T2, T3)
            Dim onClause = New OnClause(Of T1, T2, T3)(JoinInfo, joinType) With {
                    .ConnectionString = ConnectionString
                    }
            Return onClause
        End Function

        Public Iterator Function Results() As IEnumerable(Of Tuple(Of T1, T2)) _
            Implements ICombinedRepository(Of T1, T2).Results
            Dim dt = BaseResults()

            For Each row As DataRow In dt.Rows
                Dim t1Inst = New T1()
                Dim t2Inst = New T2()
                Dim tuple = New Tuple(Of T1, T2)(
                    CType(t1Inst.SetValues(row, $"{t1Inst.EntityName}{SqlParamSeparator}"), T1),
                    CType(t2Inst.SetValues(row, $"{t2Inst.EntityName}{SqlParamSeparator}"), T2))
                Yield tuple
            Next
        End Function
    End Class

    Public Class CombinedRepository (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New})
        Inherits CombinedRepositoryBase
        Implements ICombinedRepository(Of T1, T2, T3)

        Public Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause)
        End Sub

        Public Function InnerJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4) _
            Implements ICombinedRepository(Of T1, T2, T3).InnerJoin
            Return Join (Of T4)(JoinType.Inner)
        End Function

        Public Function LeftJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4) _
            Implements ICombinedRepository(Of T1, T2, T3).LeftJoin
            Return Join (Of T4)(JoinType.Left)
        End Function

        Public Function RightJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4) _
            Implements ICombinedRepository(Of T1, T2, T3).RightJoin
            Return Join (Of T4)(JoinType.Right)
        End Function

        Public Function FullJoin (Of T4 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4) _
            Implements ICombinedRepository(Of T1, T2, T3).FullJoin
            Return Join (Of T4)(JoinType.Full)
        End Function

        Private Function Join (Of T4 As {IBaseModel, New})(ByVal joinType As JoinType) As IOnClause(Of T1, T2, T3, T4)
            Dim onClause = New OnClause(Of T1, T2, T3, T4)(JoinInfo, joinType) With {
                    .ConnectionString = ConnectionString
                    }
            Return onClause
        End Function

        Public Iterator Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3)) _
            Implements ICombinedRepository(Of T1, T2, T3).Results
            Dim dt = BaseResults()

            For Each row As DataRow In dt.Rows
                Dim t1Inst = New T1()
                Dim t2Inst = New T2()
                Dim t3Inst = New T3()
                Dim tuple = New Tuple(Of T1, T2, T3)(
                    CType(t1Inst.SetValues(row, $"{t1Inst.EntityName}{SqlParamSeparator}"), T1),
                    CType(t2Inst.SetValues(row, $"{t2Inst.EntityName}{SqlParamSeparator}"), T2),
                    CType(t3Inst.SetValues(row, $"{t3Inst.EntityName}{SqlParamSeparator}"), T3))
                Yield tuple
            Next
        End Function
    End Class

    Public Class CombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New})
        Inherits CombinedRepositoryBase
        Implements ICombinedRepository(Of T1, T2, T3, T4)

        Public Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause)
        End Sub

        Public Function InnerJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5) _
            Implements ICombinedRepository(Of T1, T2, T3, T4).InnerJoin
            Return Join (Of T5)(JoinType.Inner)
        End Function

        Public Function LeftJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5) _
            Implements ICombinedRepository(Of T1, T2, T3, T4).LeftJoin
            Return Join (Of T5)(JoinType.Left)
        End Function

        Public Function RightJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5) _
            Implements ICombinedRepository(Of T1, T2, T3, T4).RightJoin
            Return Join (Of T5)(JoinType.Right)
        End Function

        Public Function FullJoin (Of T5 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5) _
            Implements ICombinedRepository(Of T1, T2, T3, T4).FullJoin
            Return Join (Of T5)(JoinType.Full)
        End Function

        Private Function Join (Of T5 As {IBaseModel, New})(ByVal joinType As JoinType) _
            As IOnClause(Of T1, T2, T3, T4, T5)
            Dim onClause = New OnClause(Of T1, T2, T3, T4, T5)(JoinInfo, joinType) With {
                    .ConnectionString = ConnectionString
                    }
            Return onClause
        End Function

        Public Iterator Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4)) _
            Implements ICombinedRepository(Of T1, T2, T3, T4).Results
            Dim dt = BaseResults()

            For Each row As DataRow In dt.Rows
                Dim t1Inst = New T1()
                Dim t2Inst = New T2()
                Dim t3Inst = New T3()
                Dim t4Inst = New T4()
                Dim tuple = New Tuple(Of T1, T2, T3, T4)(
                    CType(t1Inst.SetValues(row, $"{t1Inst.EntityName}{SqlParamSeparator}"), T1),
                    CType(t2Inst.SetValues(row, $"{t2Inst.EntityName}{SqlParamSeparator}"), T2),
                    CType(t3Inst.SetValues(row, $"{t3Inst.EntityName}{SqlParamSeparator}"), T3),
                    CType(t4Inst.SetValues(row, $"{t4Inst.EntityName}{SqlParamSeparator}"), T4))
                Yield tuple
            Next
        End Function
    End Class

    Public Class CombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New})
        Inherits CombinedRepositoryBase
        Implements ICombinedRepository(Of T1, T2, T3, T4, T5)

        Public Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause)
        End Sub

        Public Function InnerJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5).InnerJoin
            Return Join (Of T6)(JoinType.Inner)
        End Function

        Public Function LeftJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5).LeftJoin
            Return Join (Of T6)(JoinType.Left)
        End Function

        Public Function RightJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5).RightJoin
            Return Join (Of T6)(JoinType.Right)
        End Function

        Public Function FullJoin (Of T6 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5).FullJoin
            Return Join (Of T6)(JoinType.Full)
        End Function

        Private Function Join (Of T6 As {IBaseModel, New})(ByVal joinType As JoinType) _
            As IOnClause(Of T1, T2, T3, T4, T5, T6)
            Dim onClause = New OnClause(Of T1, T2, T3, T4, T5, T6)(JoinInfo, joinType) With {
                    .ConnectionString = ConnectionString
                    }
            Return onClause
        End Function

        Public Iterator Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4, T5)) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5).Results
            Dim dt = BaseResults()

            For Each row As DataRow In dt.Rows
                Dim t1Inst = New T1()
                Dim t2Inst = New T2()
                Dim t3Inst = New T3()
                Dim t4Inst = New T4()
                Dim t5Inst = New T5()
                Dim tuple = New Tuple(Of T1, T2, T3, T4, T5)(
                    CType(t1Inst.SetValues(row, $"{t1Inst.EntityName}{SqlParamSeparator}"), T1),
                    CType(t2Inst.SetValues(row, $"{t2Inst.EntityName}{SqlParamSeparator}"), T2),
                    CType(t3Inst.SetValues(row, $"{t3Inst.EntityName}{SqlParamSeparator}"), T3),
                    CType(t4Inst.SetValues(row, $"{t4Inst.EntityName}{SqlParamSeparator}"), T4),
                    CType(t5Inst.SetValues(row, $"{t5Inst.EntityName}{SqlParamSeparator}"), T5))
                Yield tuple
            Next
        End Function
    End Class

    Public Class CombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New})
        Inherits CombinedRepositoryBase
        Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6)

        Public Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause)
        End Sub

        Public Function InnerJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6).InnerJoin
            Return Join (Of T7)(JoinType.Inner)
        End Function

        Public Function LeftJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6).LeftJoin
            Return Join (Of T7)(JoinType.Left)
        End Function

        Public Function RightJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6).RightJoin
            Return Join (Of T7)(JoinType.Right)
        End Function

        Public Function FullJoin (Of T7 As {IBaseModel, New})() As IOnClause(Of T1, T2, T3, T4, T5, T6, T7) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6).FullJoin
            Return Join (Of T7)(JoinType.Full)
        End Function

        Private Function Join (Of T7 As {IBaseModel, New})(ByVal joinType As JoinType) _
            As IOnClause(Of T1, T2, T3, T4, T5, T6, T7)
            Dim onClause = New OnClause(Of T1, T2, T3, T4, T5, T6, T7)(JoinInfo, joinType) With {
                    .ConnectionString = ConnectionString
                    }
            Return onClause
        End Function

        Public Iterator Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4, T5, T6)) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6).Results
            Dim dt = BaseResults()

            For Each row As DataRow In dt.Rows
                Dim t1Inst = New T1()
                Dim t2Inst = New T2()
                Dim t3Inst = New T3()
                Dim t4Inst = New T4()
                Dim t5Inst = New T5()
                Dim t6Inst = New T6()
                Dim tuple = New Tuple(Of T1, T2, T3, T4, T5, T6)(
                    CType(t1Inst.SetValues(row, $"{t1Inst.EntityName}{SqlParamSeparator}"), T1),
                    CType(t2Inst.SetValues(row, $"{t2Inst.EntityName}{SqlParamSeparator}"), T2),
                    CType(t3Inst.SetValues(row, $"{t3Inst.EntityName}{SqlParamSeparator}"), T3),
                    CType(t4Inst.SetValues(row, $"{t4Inst.EntityName}{SqlParamSeparator}"), T4),
                    CType(t5Inst.SetValues(row, $"{t5Inst.EntityName}{SqlParamSeparator}"), T5),
                    CType(t6Inst.SetValues(row, $"{t6Inst.EntityName}{SqlParamSeparator}"), T6))
                Yield tuple
            Next
        End Function
    End Class

    Public Class CombinedRepository _
        (Of T1 As {IBaseModel, New}, T2 As {IBaseModel, New}, T3 As {IBaseModel, New}, T4 As {IBaseModel, New},
            T5 As {IBaseModel, New}, T6 As {IBaseModel, New}, T7 As {IBaseModel, New})
        Inherits CombinedRepositoryBase
        Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6, T7)

        Public Sub New(ByVal previousOnClause As BaseOnClause)
            MyBase.New(previousOnClause)
        End Sub

        Public Iterator Function Results() As IEnumerable(Of Tuple(Of T1, T2, T3, T4, T5, T6, T7)) _
            Implements ICombinedRepository(Of T1, T2, T3, T4, T5, T6, T7).Results
            Dim dt = BaseResults()

            For Each row As DataRow In dt.Rows
                Dim t1Inst = New T1()
                Dim t2Inst = New T2()
                Dim t3Inst = New T3()
                Dim t4Inst = New T4()
                Dim t5Inst = New T5()
                Dim t6Inst = New T6()
                Dim t7Inst = New T7()
                Dim tuple = New Tuple(Of T1, T2, T3, T4, T5, T6, T7)(
                    CType(t1Inst.SetValues(row, $"{t1Inst.EntityName}{SqlParamSeparator}"), T1),
                    CType(t2Inst.SetValues(row, $"{t2Inst.EntityName}{SqlParamSeparator}"), T2),
                    CType(t3Inst.SetValues(row, $"{t3Inst.EntityName}{SqlParamSeparator}"), T3),
                    CType(t4Inst.SetValues(row, $"{t4Inst.EntityName}{SqlParamSeparator}"), T4),
                    CType(t5Inst.SetValues(row, $"{t5Inst.EntityName}{SqlParamSeparator}"), T5),
                    CType(t6Inst.SetValues(row, $"{t6Inst.EntityName}{SqlParamSeparator}"), T6),
                    CType(t7Inst.SetValues(row, $"{t7Inst.EntityName}{SqlParamSeparator}"), T7))
                Yield tuple
            Next
        End Function
    End Class


#end region

    Public MustInherit Class RepositoryDataAccess
        Protected Logger As Action(Of Exception)
        Protected ConnectionString As String

        Protected Sub New(ByVal connectionString As String)
            Logger = Sub(exception)
            End Sub

            Me.ConnectionString = connectionString
        End Sub

        Protected Function CreateCommand(ByVal cn As SqlConnection, ByVal command As String) As SqlCommand
            Dim cmd = New SqlCommand With {
                    .Connection = cn,
                    .CommandType = CommandType.Text,
                    .CommandText = command
                    }
            Return cmd
        End Function

        Protected Function HasInjection(ByVal query As String) As Boolean
            Dim isSqlInjection = False
            Dim sqlCheckList As String() = {"--", ";--", "/*", "*/"}
            Dim checkString = query.Replace("'", "''")

            For i = 0 To sqlCheckList.Length - 1
                If (checkString.IndexOf(sqlCheckList(i), StringComparison.OrdinalIgnoreCase) < 0) Then Continue For
                isSqlInjection = True
                Exit For
            Next

            Return isSqlInjection
        End Function

        Protected Function ToDataTable(ByVal cmd As SqlCommand, ByVal cn As SqlConnection, <Out> ByRef dt As DataTable) As Boolean
            Dim isSuccess = True
            If HasInjection(cmd.CommandText) Then Throw New Exception("Sql Injection attempted. Aborted")
            cn.Open()
            dt = ToDataTable(cmd)
            cn.Close()
            If dt Is Nothing OrElse dt.Rows.Count = 0 Then isSuccess = False
            Return isSuccess
        End Function

        Protected Function ToDataTable(ByVal cmd As SqlCommand) As DataTable
            Try
                Dim da = New SqlDataAdapter(cmd)
                Dim dt = New DataTable()
                da.Fill(dt)
                Return dt
            Catch ex As Exception
                Logger(ex)
                Return Nothing
            End Try
        End Function
    End Class


    Public MustInherit Class BaseRepository (Of T As {IBaseModel, New})
        Inherits RepositoryDataAccess
        Implements IBaseRepository(Of T)

        Private ReadOnly _schema As String
        Private ReadOnly _tableName As String
        Private Property Columns As List(Of ColumnDefinition)

        Protected Sub New(connectionString As String, logMethod As Action(Of Exception),
                          schema As String, table As String, columns As List(Of ColumnDefinition))

            MyBase.New(connectionString)
            _schema = schema
            _tableName = table
            Logger = If(logMethod, (Sub(exception) 
            End Sub)
                        )
            Me.Columns = columns
            Dim sql =
                    $"SELECT COUNT(*)
                            FROM INFORMATION_SCHEMA.COLUMNS
                            WHERE TABLE_NAME = '{ _
                    table}' AND TABLE_SCHEMA = '{schema}'"

            Using cn = New SqlConnection(ConnectionString)

                Using cmd = CreateCommand(cn, sql)

                    Try
                        cn.Open()
                        Dim count = CInt(cmd.ExecuteScalar())
                        If count <> columns.Count Then
                            Throw New Exception(
                                "Repository Definition does not match Database. Please re-run the code generator to get a new repository")
                        End If
                    Finally
                        cn.Close()
                    End Try
                End Using
            End Using
        End Sub

        Public Function RecordCount() As Long
            Dim query = BuildWhereQuery({New ColumnDefinition("'x'")})
            Dim dt = Where(query, "1=1")
            Return If(dt Is Nothing, 0, dt.Rows.Count)
        End Function

        Public Function GetAll() As IEnumerable(Of T) Implements IBaseRepository(Of T).GetAll
            Return Where("1=1")
        End Function

        Public MustOverride Function Create(item As T) As Boolean Implements IBaseRepository(Of T).Create

        Public MustOverride Function BulkCreate(items As List(Of T)) As Boolean _
            Implements IBaseRepository(Of T).BulkCreate

        Public MustOverride Function BulkCreate(ParamArray items As T()) As Boolean _
            Implements IBaseRepository(Of T).BulkCreate

        Protected MustOverride Function ToItem(row As DataRow) As T

        Protected Friend Function WhereQuery() As String
            Return BuildWhereQuery(Columns)
        End Function

        Private Function BuildWhereQuery(ByVal columns As IEnumerable(Of ColumnDefinition)) As String
            Dim sb = New StringBuilder()
            sb.AppendLine("SELECT ")
            Dim columnArray = columns.ToArray()

            If columnArray.Any() Then

                For Each column In columnArray
                    sb.Append(column.ColumnName)
                    If column IsNot columnArray.Last() Then sb.Append(", ")
                Next
            End If

            sb.Append($" FROM [{_schema}].[{_tableName}]")
            Return sb.ToString()
        End Function

        Public Function Where(col As String, comparison As Comparison, val As Object) As Where(Of T) _
            Implements IBaseRepository(Of T).Where
            Return Where(col, comparison, val, val.[GetType]())
        End Function

        Public Function Where(col As String, comparison As Comparison, val As Object,
                              valueType As Type) As Where(Of T) Implements IBaseRepository(Of T).Where
            Return New Where(Of T)(Me, col, comparison, val, valueType)
        End Function

        Public Function Where(ByVal query As String) As IEnumerable(Of T) Implements IBaseRepository(Of T).Where
            Dim dt = Where(WhereQuery(), query)
            Return If(dt Is Nothing, New T(- 1) {}, ToItems(dt))
        End Function

        Private Function Where(columnPart As String, filterPart As String) As DataTable
            If HasInjection(columnPart) OrElse HasInjection(filterPart) Then _
                Throw New Exception("Sql Injection attempted. Aborted")

            Using cn = New SqlConnection(ConnectionString)

                Using cmd = CreateCommand(cn, $"{columnPart} WHERE {filterPart}")
                    If HasInjection(cmd.CommandText) Then Throw New Exception("Sql Injection attempted. Aborted")
                    cn.Open()
                    Dim dt = ToDataTable(cmd)
                    If dt Is Nothing Then Return Nothing
                    cn.Close()
                    Return dt
                End Using
            End Using
        End Function

        Protected Function BaseSearch(queries As List(Of QueryItem)) As IEnumerable(Of T)
            If Not queries.Any() Then Return New List(Of T)()
            Dim first = queries.First()
            Dim whereQuery = Where(first.DbColumnName, Comparison.Equals, first.Value, first.DataType)

            If queries.Count > 1 Then
                whereQuery = queries.Skip(1).Aggregate(whereQuery,
                                                       Function(current, query) _
                                                          current.[And](query.DbColumnName, Comparison.Equals,
                                                                        query.Value, first.DataType))
            End If

            Return whereQuery.Results()
        End Function

        Protected Function BaseCreate(ParamArray values As Object()) As Dictionary(Of String, Object)
            Dim returnIds = New Dictionary(Of String, Object)()

            Using cn = New SqlConnection(ConnectionString)
                Dim sb = New StringBuilder()
                Dim pkCols = Columns.Where(Function(x) x.PrimaryKey).ToList()

                If Columns.Any(Function(x) x.PrimaryKey) Then
                    sb.AppendLine("DECLARE @tempo TABLE (")

                    For Each pk In pkCols
                        sb.Append($"[{pk.ColumnName}]  {pk.SqlDataTypeText}")
                        sb.AppendLine(If(pk IsNot pkCols(pkCols.Count - 1), ",", String.Empty))
                    Next

                    sb.AppendLine(")")
                End If

                sb.AppendLine($"INSERT [{_schema}].[{_tableName}] (")
                Dim toCreate =
                        Columns.Where(Function(x) Not x.PrimaryKey OrElse x.PrimaryKey AndAlso Not x.Identity).ToList()

                For Each createColumn In toCreate
                    sb.Append($"[{createColumn.ColumnName}]")
                    sb.AppendLine(If(createColumn IsNot Columns.Last(), ",", ")"))
                Next

                If Columns.Any(Function(x) x.PrimaryKey) Then
                    sb.Append("OUTPUT ")

                    For Each pk In pkCols
                        sb.Append($"[Inserted].[{pk.ColumnName}] ")
                        sb.AppendLine(If(pk IsNot pkCols(pkCols.Count - 1), ",", String.Empty))
                    Next

                    sb.AppendLine("INTO @tempo ")
                End If

                sb.AppendLine("VALUES (")
                Dim valueCols =
                        Columns.Where(Function(x) Not x.PrimaryKey OrElse (x.PrimaryKey AndAlso Not x.Identity)).ToList()

                For Each createColumn In valueCols
                    sb.Append("@" & createColumn.ColumnName)
                    sb.AppendLine(If(createColumn IsNot valueCols.Last(), ",", ")"))
                Next

                If Columns.Any(Function(x) x.PrimaryKey) Then
                    sb.AppendLine("SELECT * FROM @tempo")
                End If

                Dim sql = sb.ToString()
                If HasInjection(sql) Then Throw New Exception("Sql Injection attempted. Aborted")

                Using cmd = CreateCommand(cn, sql)

                    For i = 0 To Columns.Count - 1
                        Dim createColumn = Columns(i)
                        If createColumn.PrimaryKey AndAlso (Not createColumn.PrimaryKey OrElse createColumn.Identity) _
                            Then Continue For
                        Dim parameter = cmd.Parameters.Add(createColumn.ColumnName, createColumn.SqlDbType)
                        parameter.Value =
                            If(values(i) IsNot Nothing,
                               If (values(i).GetType() = GetType(XmlDocument), CType(values(i), XmlDocument).InnerXml,
                                   values(i)), DBNull.Value)
                    Next

                    Dim dt As DataTable = New DataTable()
                    Dim isSuccess = ToDataTable(cmd, cn, dt)
                    If Not isSuccess Then Return returnIds

                    If dt.Rows.Count > 0 Then

                        For i = 0 To dt.Columns.Count - 1
                            Dim dataColumn = dt.Columns(i)
                            returnIds.Add(dataColumn.ColumnName, dt.Rows(0)(i))
                        Next
                    End If
                End Using
            End Using

            Return returnIds
        End Function

        Protected Function BulkInsert(dt As DataTable, tableName As String) As Boolean
            Try
                Using cn = New SqlConnection(ConnectionString)
                    cn.Open()

                    Using bulkCopy =
                        New SqlBulkCopy(cn,
                                        SqlBulkCopyOptions.TableLock Or SqlBulkCopyOptions.FireTriggers Or
                                        SqlBulkCopyOptions.UseInternalTransaction, Nothing) With {
                                            .DestinationTableName = tableName
                                            }

                        For Each dataColumn As DataColumn In dt.Columns
                            bulkCopy.ColumnMappings.Add(dataColumn.ColumnName, dataColumn.ColumnName)
                        Next

                        bulkCopy.WriteToServer(dt)
                    End Using

                    cn.Close()
                    Return True
                End Using

            Catch ex As Exception
                Logger(ex)
                Return False
            End Try
        End Function

        Protected Function BulkInsert(dt As DataTable) As Boolean
            Return BulkInsert(dt, $"[{_schema}].[{_tableName}]")
        End Function

        Protected Function BaseUpdate(dirtyColumns As List(Of String), ParamArray values As Object()) As Boolean
            Dim isSuccess As Boolean
            Dim sb = New StringBuilder()
            sb.AppendLine($"UPDATE [{_schema}].[{_tableName}] SET")
            Dim nonPkCols = Columns.Where(Function(x) Not x.PrimaryKey).ToArray()

            For Each col In nonPkCols.Where(Function(x) dirtyColumns.Contains(x.ColumnName))
                sb.Append($"[{col.ColumnName}] = @{col.ColumnName}")
                sb.AppendLine(If(col IsNot nonPkCols.Last(Function(x) dirtyColumns.Contains(x.ColumnName)), ",", ""))
            Next

            sb.AppendLine("WHERE")
            Dim pkCols = Columns.Where(Function(x) x.PrimaryKey).ToArray()

            For Each pk In pkCols
                sb.AppendLine(
                    If _
                                 (pk Is pkCols.First(), $"[{pk.ColumnName}] = @{pk.ColumnName}",
                                  $"AND [{pk.ColumnName}] = @{pk.ColumnName}"))
            Next

            Dim sql = sb.ToString()
            If HasInjection(sql) Then Throw New Exception("Sql Injection attempted. Aborted")

            Using cn = New SqlConnection(ConnectionString)

                Using cmd = CreateCommand(cn, sql)

                    For i = 0 To Columns.Count - 1
                        Dim updateColumn = Columns(i)
                        If Not dirtyColumns.Contains(updateColumn.ColumnName) AndAlso Not updateColumn.PrimaryKey Then _
                            Continue For
                        Dim parameter = cmd.Parameters.Add(updateColumn.ColumnName, updateColumn.SqlDbType)

                        If updateColumn.PrimaryKey Then
                            parameter.Value = values(i)
                        Else
                            parameter.Value =
                                If _
                                    (dirtyColumns.Contains(updateColumn.ColumnName), If(values(i), DBNull.Value),
                                     values(i))
                        End If
                    Next

                    isSuccess = NoneQuery(cn, cmd)
                End Using
            End Using

            Return isSuccess
        End Function

        Protected Function BaseDelete(deleteColumn As DeleteColumn) As Boolean
            Dim isSuccess As Boolean

            Using cn = New SqlConnection(ConnectionString)
                Dim sb = New StringBuilder()
                sb.Append($"DELETE [{_schema}].[{_tableName}] WHERE ")
                sb.Append($"[{deleteColumn.ColumnName}] = @{deleteColumn.ColumnName}")
                Dim sql = sb.ToString()
                If HasInjection(sql) Then Throw New Exception("Sql Injection attempted. Aborted")

                Using cmd = CreateCommand(cn, sql)
                    Dim parameter = cmd.Parameters.Add(deleteColumn.ColumnName, deleteColumn.SqlDbType)
                    parameter.Value = deleteColumn.Data
                    isSuccess = NoneQuery(cn, cmd)
                End Using
            End Using

            Return isSuccess
        End Function

        Protected Function BaseDelete(columnName As String, dataValues As List(Of Object)) As Boolean
            Dim isSuccess As Boolean

            Using cn = New SqlConnection(ConnectionString)
                Dim sb = New StringBuilder()
                sb.Append($"DELETE [{_schema}].[{_tableName}] WHERE [{columnName}] IN (")

                For Each dataValue In dataValues
                    sb.Append(dataValue)
                    If dataValue <> dataValues.Last() Then sb.Append(", ")
                Next

                sb.Append(")")
                Dim sql = sb.ToString()
                If HasInjection(sql) Then Throw New Exception("Sql Injection attempted. Aborted")

                Using cmd = CreateCommand(cn, sql)
                    isSuccess = NoneQuery(cn, cmd)
                End Using
            End Using

            Return isSuccess
        End Function

        Protected Function BaseMerge(mergeData As List(Of Object())) As Boolean
            Dim tempTableName = "staging" & DateTime.Now.Ticks

            Try
                Dim dt = New DataTable()

                For Each mergeColumn In Columns
                    dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType)
                    If Not mergeColumn.PrimaryKey Then _
                        dt.Columns.Add(mergeColumn.ColumnName & "Changed", GetType(Boolean))
                Next

                For Each m In mergeData
                    dt.Rows.Add(m)
                Next

                CreateStagingTable(tempTableName)
                BulkInsert(dt, tempTableName)

                Using cn = New SqlConnection(ConnectionString)
                    Dim mergeSql = New StringBuilder()
                    mergeSql.AppendLine($"MERGE INTO [{_schema}].[{_tableName}] AS [Target]")
                    mergeSql.AppendLine($"USING {tempTableName} AS Source")
                    mergeSql.AppendLine("ON")
                    Dim pks = Columns.Where(Function(x) x.PrimaryKey).ToArray()

                    For Each pk In pks
                        If pk IsNot pks.First() Then mergeSql.Append("AND ")
                        mergeSql.AppendLine($"[Target].[{pk.ColumnName}] = [Source].[{pk.ColumnName}]")
                    Next

                    mergeSql.AppendLine("WHEN MATCHED THEN UPDATE SET")
                    Dim nonpks = Columns.Where(Function(x) Not x.PrimaryKey).ToArray()

                    For Each mergeColumn In nonpks
                        mergeSql.Append(
                            $"[Target].[{mergeColumn.ColumnName}] = CASE WHEN [Source].[{mergeColumn.ColumnName _
                                           }Changed] = 1 THEN [Source].[{mergeColumn.ColumnName}] ELSE [Target].[{ _
                                           mergeColumn.ColumnName}] END")
                        mergeSql.AppendLine(If(mergeColumn IsNot nonpks.Last(), ",", Environment.NewLine))
                    Next

                    mergeSql.AppendLine("WHEN NOT MATCHED THEN INSERT (")
                    mergeSql.AppendLine(
                        String.Join(",",
                                    Columns.Where(Function(x) Not x.Identity).[Select](Function(x) $"[{x.ColumnName}]").
                                       ToArray()) & ")")
                    mergeSql.AppendLine("VALUES (")
                    mergeSql.AppendLine(
                        String.Join(",",
                                    Columns.Where(Function(x) Not x.Identity).[Select](
                                        Function(x) $"[Source].[{x.ColumnName}]").ToArray()) & ");")
                    mergeSql.AppendLine($"DROP TABLE {tempTableName}")
                    Dim sql = mergeSql.ToString()
                    If HasInjection(sql) Then Throw New Exception("Sql Injection attempted. Aborted")
                    Dim cmd = New SqlCommand With {
                            .Connection = cn,
                            .CommandType = CommandType.Text,
                            .CommandText = sql
                            }

                    With cmd
                        .Connection = cn
                        .CommandType = CommandType.Text
                        .CommandText = sql
                    End With
                    cn.Open()
                    cmd.ExecuteNonQuery()
                    cmd.Dispose()
                    Dim dropCmd = New SqlCommand With {
                            .Connection = cn,
                            .CommandType = CommandType.StoredProcedure,
                            .CommandText = "dbo.DropTmpTable"
                            }
                    dropCmd.Parameters.AddWithValue("table", tempTableName)
                    dropCmd.ExecuteNonQuery()
                    dropCmd.Dispose()
                    cn.Close()
                    Return True
                End Using

            Catch ex As Exception
                Logger(ex)

                Using cn = New SqlConnection(ConnectionString)
                    Dim cmd = New SqlCommand With {
                            .Connection = cn,
                            .CommandType = CommandType.Text,
                            .CommandText = $"DROP TABLE {tempTableName}"
                            }

                    Try
                        cmd.ExecuteNonQuery()
                    Catch ex2 As Exception
                        Logger(ex2)
                    End Try
                End Using

                Return False
            End Try
        End Function

        Public Function InnerJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T, T2)
            Dim onClause = New OnClause(Of T, T2)(ConnectionString, JoinType.Inner)
            Return onClause
        End Function

        Public Function LeftJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T, T2)
            Dim onClause = New OnClause(Of T, T2)(ConnectionString, JoinType.Left)
            Return onClause
        End Function

        Public Function RightJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T, T2)
            Dim onClause = New OnClause(Of T, T2)(ConnectionString, JoinType.Right)
            Return onClause
        End Function

        Public Function FullJoin (Of T2 As {IBaseModel, New})() As IOnClause(Of T, T2)
            Dim onClause = New OnClause(Of T, T2)(ConnectionString, JoinType.Full)
            Return onClause
        End Function

        Protected Iterator Function ToItems(table As DataTable) As IEnumerable(Of T)
            For Each row As DataRow In table.Rows
                Dim item = Nothing

                Try
                    item = ToItem(row)
                Catch ex As Exception
                    Logger(ex)
                End Try

                Yield item
            Next
        End Function

        Protected Function NoneQuery(cn As SqlConnection, cmd As SqlCommand) As Boolean
            If HasInjection(cmd.CommandText) Then Throw New Exception("Sql Injection attempted. Aborted")
            Dim isSuccess = True
            cn.Open()

            Try
                cmd.ExecuteNonQuery()
            Catch ex As Exception
                Logger(ex)
                isSuccess = False
            End Try

            cn.Close()
            Return isSuccess
        End Function

        Protected Function GetBoolean(row As DataRow, fieldName As String) As Boolean
            Return If(row.GetValue (Of Boolean)(fieldName), False)
        End Function

        Protected Function GetNullableBoolean(row As DataRow, fieldName As String) As Boolean?
            Return row.GetValue (Of Boolean)(fieldName)
        End Function

        Protected Function GetInt16(row As DataRow, fieldName As String) As Short
            Return If(row.GetValue (Of Short)(fieldName), Nothing)
        End Function

        Protected Function GetNullableInt16(row As DataRow, fieldName As String) As Short?
            Return row.GetValue (Of Short)(fieldName)
        End Function

        Protected Function GetInt32(row As DataRow, fieldName As String) As Integer
            Return If(row.GetValue (Of Integer)(fieldName), Nothing)
        End Function

        Protected Function GetNullableInt32(row As DataRow, fieldName As String) As Integer?
            Return row.GetValue (Of Integer)(fieldName)
        End Function

        Protected Function GetInt64(row As DataRow, fieldName As String) As Long
            Return If(row.GetValue (Of Long)(fieldName), Nothing)
        End Function

        Protected Function GetNullableInt64(row As DataRow, fieldName As String) As Long?
            Return row.GetValue (Of Long)(fieldName)
        End Function

        Protected Function GetDecimal(row As DataRow, fieldName As String) As Decimal
            Return If(row.GetValue (Of Decimal)(fieldName), Nothing)
        End Function

        Protected Function GetNullableDecimal(row As DataRow, fieldName As String) As Decimal?
            Return row.GetValue (Of Decimal)(fieldName)
        End Function

        Protected Function GetDouble(row As DataRow, fieldName As String) As Double
            Return If(row.GetValue (Of Double)(fieldName), Nothing)
        End Function

        Protected Function GetNullableDouble(row As DataRow, fieldName As String) As Double?
            Return row.GetValue (Of Double)(fieldName)
        End Function

        Protected Function GetDateTime(row As DataRow, fieldName As String) As DateTime
            Return If(row.GetValue (Of DateTime)(fieldName), Nothing)
        End Function

        Protected Function GetNullableDateTime(row As DataRow, fieldName As String) As DateTime?
            Return row.GetValue (Of DateTime)(fieldName)
        End Function

        Protected Function GetByte(row As DataRow, fieldName As String) As Byte
            Return row(fieldName)
        End Function

        Protected Function GetNullableByte(ByVal row As DataRow, ByVal fieldName As String) As Byte?
            Return row(fieldName)
        End Function

        Protected Function GetByteArray(ByVal row As DataRow, ByVal fieldName As String) As Byte()
            Return TryCast(row(fieldName), Byte())
        End Function

        Protected Function GetDateTimeOffset(row As DataRow, fieldName As String) As DateTimeOffset
            Return If(row.GetValue (Of DateTimeOffset)(fieldName), Nothing)
        End Function

        Protected Function GetNullableDateTimeOffset(row As DataRow, fieldName As String) As DateTimeOffset?
            Return row.GetValue (Of DateTimeOffset)(fieldName)
        End Function

        Protected Function GetGuid(row As DataRow, fieldName As String) As Guid
            Return If(row.GetValue (Of Guid)(fieldName), Guid.Empty)
        End Function

        Protected Function GetNullableGuid(row As DataRow, fieldName As String) As Guid?
            Return row.GetValue (Of Guid)(fieldName)
        End Function

        Protected Function GetTimeSpan(row As DataRow, fieldName As String) As TimeSpan
            Return If(row.GetValue (Of TimeSpan)(fieldName), Nothing)
        End Function

        Protected Function GetNullableTimeSpan(row As DataRow, fieldName As String) As TimeSpan?
            Return row.GetValue (Of TimeSpan)(fieldName)
        End Function

        Protected Function GetXmlDocument(row As DataRow, fieldName As String) As XmlDocument
            Return New XmlDocument With {
                .InnerXml = If(row.Table.Columns.Contains(fieldName), row.GetText(fieldName), "")
                }
        End Function

        Protected Function GetString(row As DataRow, fieldName As String) As String
            Return If(row.Table.Columns.Contains(fieldName), row.GetText(fieldName), Nothing)
        End Function

        Protected Sub CreateStagingTable(tempTableName As String,
                                         Optional onlyPrimaryKeys As Boolean = False)
            Dim stagingSqlBuilder = New StringBuilder()
            stagingSqlBuilder.AppendLine("CREATE TABLE " & tempTableName & " (")

            For Each mergeColumn In _
                Columns.Where(Function(x) onlyPrimaryKeys AndAlso x.PrimaryKey OrElse Not onlyPrimaryKeys)
                stagingSqlBuilder.Append($"[{mergeColumn.ColumnName}] {mergeColumn.SqlDataTypeText} NULL")

                If Not mergeColumn.PrimaryKey Then
                    stagingSqlBuilder.AppendLine(",")
                    stagingSqlBuilder.Append($"[{mergeColumn.ColumnName}Changed] [BIT] NOT NULL")
                End If

                stagingSqlBuilder.AppendLine(If(mergeColumn IsNot Columns(Columns.Count - 1), ",", ")"))
            Next

            Dim stagingSql = stagingSqlBuilder.ToString()
            If HasInjection(stagingSql) Then Throw New Exception("Sql Injection attempted. Aborted")

            Using cn = New SqlConnection(ConnectionString)
                Dim cmd = New SqlCommand With {
                        .Connection = cn,
                        .CommandType = CommandType.Text,
                        .CommandText = stagingSql
                        }
                cn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Sub
    End Class

    Module Ext
        <Extension>
        Function GetValue (Of T As Structure)(row As DataRow, columnName As String) As T?
            If row.IsNull(columnName) OrElse Not row.Table.Columns.Contains(columnName) Then Return Nothing
            Return row(columnName)
        End Function

        <Extension>
        Function GetText(row As DataRow, columnName As String) As String
            If row.IsNull(columnName) OrElse Not row.Table.Columns.Contains(columnName) Then Return Nothing
            Return If(TryCast(row(columnName), String), Nothing)
        End Function
    End Module
End Namespace