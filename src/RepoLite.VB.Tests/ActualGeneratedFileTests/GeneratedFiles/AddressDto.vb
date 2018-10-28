Imports RepoLite.VB.Tests.MODELNAMESPACE.Base
Imports RepoLite.VB.Tests.REPOSITORYNAMESPACE.Base

Namespace NS.Models
    Partial Public Class Address
        Inherits BaseModel

        Public Overrides ReadOnly Property EntityName As String = "Address"

        Private _id As Int32
        Private _anotherId As String
        Private _personId As Int32
        Private _line1 As String
        Private _line2 As String
        Private _line3 As String
        Private _line4 As String
        Private _postCode As String
        Private _phoneNumber As String
        Private _cOUNTRY_CODE As String

        Public Overridable Property Id As Int32
            Get
                Return _id
            End Get
            Set(value As Int32)
                SetValue(_id, value)
            End Set
        End Property

        Public Overridable Property AnotherId As String
            Get
                Return _anotherId
            End Get
            Set(value As String)
                SetValue(_anotherId, value)
            End Set
        End Property

        Public Overridable Property PersonId As Int32
            Get
                Return _personId
            End Get
            Set(value As Int32)
                SetValue(_personId, value)
            End Set
        End Property

        Public Overridable Property Line1 As String
            Get
                Return _line1
            End Get
            Set(value As String)
                SetValue(_line1, value)
            End Set
        End Property

        Public Overridable Property Line2 As String
            Get
                Return _line2
            End Get
            Set(value As String)
                SetValue(_line2, value)
            End Set
        End Property

        Public Overridable Property Line3 As String
            Get
                Return _line3
            End Get
            Set(value As String)
                SetValue(_line3, value)
            End Set
        End Property

        Public Overridable Property Line4 As String
            Get
                Return _line4
            End Get
            Set(value As String)
                SetValue(_line4, value)
            End Set
        End Property

        Public Overridable Property PostCode As String
            Get
                Return _postCode
            End Get
            Set(value As String)
                SetValue(_postCode, value)
            End Set
        End Property

        Public Overridable Property PhoneNumber As String
            Get
                Return _phoneNumber
            End Get
            Set(value As String)
                SetValue(_phoneNumber, value)
            End Set
        End Property

        Public Overridable Property COUNTRY_CODE As String
            Get
                Return _cOUNTRY_CODE
            End Get
            Set(value As String)
                SetValue(_cOUNTRY_CODE, value)
            End Set
        End Property

        Public Overrides Function Validate() As List(Of ValidationError)
            Dim validationErrors = New List(Of ValidationError)()
            If String.IsNullOrEmpty(AnotherId) Then validationErrors.Add(New ValidationError(NameOf(AnotherId), "Value cannot be null"))
            If Not String.IsNullOrEmpty(AnotherId) AndAlso AnotherId.Length > 10 Then validationErrors.Add(New ValidationError(NameOf(AnotherId), "Max length is 10"))
            If String.IsNullOrEmpty(Line1) Then validationErrors.Add(New ValidationError(NameOf(Line1), "Value cannot be null"))
            If Not String.IsNullOrEmpty(Line1) AndAlso Line1.Length > 100 Then validationErrors.Add(New ValidationError(NameOf(Line1), "Max length is 100"))
            If Not String.IsNullOrEmpty(Line2) AndAlso Line2.Length > 100 Then validationErrors.Add(New ValidationError(NameOf(Line2), "Max length is 100"))
            If Not String.IsNullOrEmpty(Line3) AndAlso Line3.Length > 100 Then validationErrors.Add(New ValidationError(NameOf(Line3), "Max length is 100"))
            If Not String.IsNullOrEmpty(Line4) AndAlso Line4.Length > 100 Then validationErrors.Add(New ValidationError(NameOf(Line4), "Max length is 100"))
            If String.IsNullOrEmpty(PostCode) Then validationErrors.Add(New ValidationError(NameOf(PostCode), "Value cannot be null"))
            If Not String.IsNullOrEmpty(PostCode) AndAlso PostCode.Length > 15 Then validationErrors.Add(New ValidationError(NameOf(PostCode), "Max length is 15"))
            If Not String.IsNullOrEmpty(PhoneNumber) AndAlso PhoneNumber.Length > 20 Then validationErrors.Add(New ValidationError(NameOf(PhoneNumber), "Max length is 20"))
            If Not String.IsNullOrEmpty(COUNTRY_CODE) AndAlso COUNTRY_CODE.Length > 2 Then validationErrors.Add(New ValidationError(NameOf(COUNTRY_CODE), "Max length is 2"))
            Return validationErrors
        End Function

        Shared Friend ReadOnly Property Columns As List(Of ColumnDefinition)
            Get
                Return New List(Of ColumnDefinition) From {
                    New ColumnDefinition("Id", GetType(System.Int32), "[INT]", SqlDbType.Int, False, True, True),
                    New ColumnDefinition("AnotherId", GetType(System.String), "[NVARCHAR](10)", SqlDbType.NVarChar, False, True, False),
                    New ColumnDefinition("PersonId", GetType(System.Int32), "[INT]", SqlDbType.Int, False, False, False),
                    New ColumnDefinition("Line1", GetType(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, False, False, False),
                    New ColumnDefinition("Line2", GetType(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, True, False, False),
                    New ColumnDefinition("Line3", GetType(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, True, False, False),
                    New ColumnDefinition("Line4", GetType(System.String), "[NVARCHAR](100)", SqlDbType.NVarChar, True, False, False),
                    New ColumnDefinition("PostCode", GetType(System.String), "[NVARCHAR](15)", SqlDbType.NVarChar, False, False, False),
                    New ColumnDefinition("PhoneNumber", GetType(System.String), "[NVARCHAR](20)", SqlDbType.NVarChar, True, False, False),
                    New ColumnDefinition("COUNTRY_CODE", GetType(System.String), "[NVARCHAR](2)", SqlDbType.NVarChar, True, False, False)
                    }
            End Get
        End Property

    End Class
End Namespace
