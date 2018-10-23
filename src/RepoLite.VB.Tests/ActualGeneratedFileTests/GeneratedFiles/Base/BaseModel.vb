Imports System.Runtime.CompilerServices

Namespace MODELNAMESPACE.Base
    Public Class ValidationError
        Public Property PropertyName As String
        Public Property [Error] As String

        Public Sub New([property] As String, [error] As String)
            PropertyName = [property]
            Me.[error] = [error]
        End Sub
    End Class

    Public MustInherit Class BaseModel
        Public MustOverride Function Validate() As List(Of ValidationError)
        Public ReadOnly DirtyColumns As List(Of String) = New List(Of String)()

        Public Sub ResetDirty()
            DirtyColumns.Clear()
        End Sub

        Protected Sub SetValue(Of T)(ByRef prop As T, value As T,
                                      <CallerMemberName> Optional propName As String = "")
            If Not DirtyColumns.Contains(propName) Then
                DirtyColumns.Add(propName)
            End If

            prop = value
        End Sub

        Public Shared Function GetDecimalPlaces(n As Decimal) As Integer
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
