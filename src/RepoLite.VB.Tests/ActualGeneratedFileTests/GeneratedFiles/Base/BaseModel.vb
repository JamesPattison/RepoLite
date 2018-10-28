Imports System.Runtime.CompilerServices
Imports RepoLite.VB.Tests.REPOSITORYNAMESPACE.Base

Namespace MODELNAMESPACE.Base
    Public Class ValidationError
        Public Property PropertyName As String
        Public Property [Error] As String

        Public Sub New(ByVal [property] As String, ByVal [error] As String)
            PropertyName = [property]
            [Error] = [error]
        End Sub
    End Class

    Public Partial Interface IBaseModel
        ReadOnly Property EntityName As String
        Function SetValues(row As DataRow, propertyPrefix as String) As IBaseModel
    End Interface

    Public MustInherit Partial Class BaseModel
        Implements IBaseModel

        Public MustOverride ReadOnly Property EntityName As String Implements IBaseModel.EntityName
        Public MustOverride Function SetValues(row As DataRow, propertyPrefix as String) As IBaseModel Implements IBaseModel.SetValues
        Public MustOverride Function Validate() As List(Of ValidationError)
        Public ReadOnly DirtyColumns As List(Of String) = New List(Of String)()

        Public Sub ResetDirty()
            DirtyColumns.Clear()
        End Sub

        Protected Sub SetValue (Of T)(ByRef prop As T, ByVal value As T,
                                      <CallerMemberName> ByVal Optional propName As String = "")
            If Not DirtyColumns.Contains(propName) Then
                DirtyColumns.Add(propName)
            End If

            prop = value
        End Sub

        Public Shared Function GetDecimalPlaces(ByVal n As Decimal) As Integer
            n = Math.Abs(n)
            n -= CInt(n)
            Dim decimalPlaces = 0

            While n > 0
                decimalPlaces += 1
                n *= 10
                n -= CInt(n)
            End While

            Return decimalPlaces
        End Function
    End Class
End Namespace
