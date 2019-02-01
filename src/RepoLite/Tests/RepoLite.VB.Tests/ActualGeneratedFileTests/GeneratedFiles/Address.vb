Imports System.Data.SqlClient
Imports RepoLite.VB.Tests.NS.Models
Imports RepoLite.VB.Tests.REPOSITORYNAMESPACE.Base

Namespace NS
    Public Class AddressKeys
        Public Property Id As Int32
        Public Property AnotherId As String

        Public Sub New()
        End Sub

        Public Sub New(id As Int32, anotherId As String)
            Me.Id = id
            Me.AnotherId = anotherId
        End Sub
    End Class

    Partial Interface IAddressRepository
        Inherits IBaseRepository(Of Address)

        Function [Get](id As Int32, anotherId As String) As Address
        Function [Get](compositeId As AddressKeys) As Address
        Function [Get](compositeIds As List(Of AddressKeys)) As IEnumerable(Of Address)
        Function [Get](ParamArray compositeIds As AddressKeys()) As IEnumerable(Of Address)
        Function Update(item As Address) As Boolean
        Function Delete(address As Address) As Boolean
        Function Delete(id As Int32, anotherId As String) As Boolean
        Function Delete(compositeId As AddressKeys) As Boolean
        Function Delete(compositeIds As IEnumerable(Of AddressKeys)) As Boolean
        Function Merge(items As List(Of Address)) As Boolean

        Function Search(Optional id As Int32? = Nothing, Optional anotherId As String = Nothing,
                        Optional personId As Int32? = Nothing, Optional line1 As String = Nothing,
                        Optional line2 As String = Nothing, Optional line3 As String = Nothing,
                        Optional line4 As String = Nothing, Optional postCode As String = Nothing,
                        Optional phoneNumber As String = Nothing, Optional cOUNTRY_CODE As String = Nothing) _
            As IEnumerable(Of Address)

        Function FindById(id As Int32) As IEnumerable(Of Address)
        Function FindById(comparison As FindComparison, id As Int32) As IEnumerable(Of Address)
        Function FindByAnotherId(anotherId As String) As IEnumerable(Of Address)

        Function FindByAnotherId(comparison As FindComparison, anotherId As String) _
            As IEnumerable(Of Address)

        Function FindByPersonId(personId As Int32) As IEnumerable(Of Address)
        Function FindByPersonId(comparison As FindComparison, personId As Int32) As IEnumerable(Of Address)
        Function FindByLine1(line1 As String) As IEnumerable(Of Address)
        Function FindByLine1(comparison As FindComparison, line1 As String) As IEnumerable(Of Address)
        Function FindByLine2(line2 As String) As IEnumerable(Of Address)
        Function FindByLine2(comparison As FindComparison, line2 As String) As IEnumerable(Of Address)
        Function FindByLine3(line3 As String) As IEnumerable(Of Address)
        Function FindByLine3(comparison As FindComparison, line3 As String) As IEnumerable(Of Address)
        Function FindByLine4(line4 As String) As IEnumerable(Of Address)
        Function FindByLine4(comparison As FindComparison, line4 As String) As IEnumerable(Of Address)
        Function FindByPostCode(postCode As String) As IEnumerable(Of Address)
        Function FindByPostCode(comparison As FindComparison, postCode As String) As IEnumerable(Of Address)
        Function FindByPhoneNumber(phoneNumber As String) As IEnumerable(Of Address)

        Function FindByPhoneNumber(comparison As FindComparison, phoneNumber As String) _
            As IEnumerable(Of Address)

        Function FindByCOUNTRY_CODE(cOUNTRY_CODE As String) As IEnumerable(Of Address)

        Function FindByCOUNTRY_CODE(comparison As FindComparison, cOUNTRY_CODE As String) _
            As IEnumerable(Of Address)
    End Interface

    Partial Public NotInheritable Class AddressRepository
        Inherits BaseRepository(Of Address)
        Implements IAddressRepository

        Public Sub New(connectionString As String)
            Me.New(connectionString, Sub(exception)
                                     End Sub)
        End Sub

        Public Sub New(connectionString As String, logMethod As Action(Of Exception))
            MyBase.New(connectionString, logMethod, "dbo", "Address", Address.Columns)
        End Sub

        Public Function [Get](id As Int32, anotherId As String) As Address _
            Implements IAddressRepository.[Get]
            Return _
                Where("Id", Comparison.Equals, id).[And]("AnotherId", Comparison.Equals, anotherId).Results().
                    FirstOrDefault()
        End Function

        Public Function [Get](compositeId As AddressKeys) As Address Implements IAddressRepository.[Get]
            Return _
                Where("Id", Comparison.Equals, compositeId.Id).[And]("AnotherId", Comparison.Equals,
                                                                     compositeId.AnotherId).Results().FirstOrDefault()
        End Function

        Public Function [Get](compositeIds As List(Of AddressKeys)) As IEnumerable(Of Address) _
            Implements IAddressRepository.[Get]
            Return [Get](compositeIds.ToArray())
        End Function

        Public Function [Get](ParamArray compositeIds As AddressKeys()) As IEnumerable(Of Address) _
            Implements IAddressRepository.[Get]
            Dim result =
                    Where("Id", Comparison.[In], compositeIds.[Select](Function(x) x.Id).ToList()).[Or]("AnotherId",
                                                                                                        Comparison.[In],
                                                                                                        compositeIds.
                                                                                                           [Select](
                                                                                                               Function _
                                                                                                                       (
                                                                                                                       x
                                                                                                                       ) _
                                                                                                                       x _
                                                                                                                       .
                                                                                                                       AnotherId) _
                                                                                                           .ToList()).
                    Results().ToArray()
            Dim filteredResults = New List(Of Address)()

            For Each compositeKey In compositeIds
                filteredResults.AddRange(
                    result.Where(Function(x) x.Id = compositeKey.Id AndAlso x.AnotherId = compositeKey.AnotherId))
            Next

            Return filteredResults
        End Function

        Public Overrides Function Create(item As Address) As Boolean Implements IBaseRepository(Of Address).Create
            If item Is Nothing Then Return False
            Dim validationErrors = item.Validate()
            If validationErrors.Any() Then Throw New ValidationException(validationErrors)
            Dim createdKeys = BaseCreate(item.Id, item.AnotherId, item.PersonId, item.Line1, item.Line2, item.Line3,
                                         item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE)
            If createdKeys.Count <> Address.Columns.AsEnumerable().Count(Function(definition) definition.PrimaryKey) Then Return False
            item.Id = CInt(createdKeys(NameOf(Address.Id)))
            item.AnotherId = CStr(createdKeys(NameOf(Address.AnotherId)))
            item.ResetDirty()
            Return True
        End Function

        Public Overrides Function BulkCreate(ParamArray items As Address()) As Boolean _
            Implements IBaseRepository(Of Address).BulkCreate
            If Not items.Any() Then Return False
            Dim validationErrors = items.SelectMany(Function(x) x.Validate()).ToList()
            If validationErrors.Any() Then Throw New ValidationException(validationErrors)
            Dim dt = New DataTable()

            For Each mergeColumn In
                Address.Columns.Where(Function(x) Not x.PrimaryKey OrElse x.PrimaryKey AndAlso Not x.Identity)
                dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType)
            Next

            For Each item In items
                dt.Rows.Add(item.AnotherId, item.PersonId, item.Line1, item.Line2, item.Line3, item.Line4, item.PostCode,
                            item.PhoneNumber, item.COUNTRY_CODE)
            Next

            Return BulkInsert(dt)
        End Function

        Public Overrides Function BulkCreate(items As List(Of Address)) As Boolean _
            Implements IBaseRepository(Of Address).BulkCreate
            Return BulkCreate(items.ToArray())
        End Function

        Public Function Update(item As Address) As Boolean Implements IAddressRepository.Update
            If item Is Nothing Then Return False
            Dim validationErrors = item.Validate()
            If validationErrors.Any() Then Throw New ValidationException(validationErrors)
            Dim success = BaseUpdate(item.DirtyColumns, item.Id, item.AnotherId, item.PersonId, item.Line1, item.Line2,
                                     item.Line3, item.Line4, item.PostCode, item.PhoneNumber, item.COUNTRY_CODE)
            If success Then item.ResetDirty()
            Return success
        End Function

        Public Function Delete(address As Address) As Boolean Implements IAddressRepository.Delete
            If address Is Nothing Then Return False
            Dim deleteColumn = New DeleteColumn("Id", address.Id, SqlDbType.Int)
            Return BaseDelete(deleteColumn)
        End Function

        Public Function Delete(id As Int32, anotherId As String) As Boolean _
            Implements IAddressRepository.Delete
            Return Delete(New Address With {
                             .Id = id,
                             .AnotherId = anotherId
                             })
        End Function

        Public Function Delete(compositeId As AddressKeys) As Boolean Implements IAddressRepository.Delete
            Return Delete(New Address With {
                             .Id = compositeId.Id,
                             .AnotherId = compositeId.AnotherId
                             })
        End Function

        Public Function Delete(compositeIds As IEnumerable(Of AddressKeys)) As Boolean _
            Implements IAddressRepository.Delete
            Dim tempTableName = $"staging{DateTime.Now.Ticks}"
            Dim dt = New DataTable()

            For Each mergeColumn In Address.Columns.Where(Function(x) x.PrimaryKey)
                dt.Columns.Add(mergeColumn.ColumnName, mergeColumn.ValueType)
            Next

            For Each compositeId In compositeIds
                dt.Rows.Add(compositeId.Id, compositeId.AnotherId)
            Next

            CreateStagingTable(tempTableName, True)
            BulkInsert(dt, tempTableName)

            Using cn = New SqlConnection(ConnectionString)

                Using _
                    cmd =
                        CreateCommand(cn,
                                      $";WITH cte AS (SELECT * FROM dbo.Address o
							WHERE EXISTS (SELECT 'x' FROM { _
                                         tempTableName _
                                         } i WHERE i.[Id] = o.[Id] AND i.[AnotherId] = o.[AnotherId]))
							DELETE FROM cte")

                    Try
                        cn.Open()
                        Return CInt(cmd.ExecuteScalar()) > 0
                    Finally
                        cn.Close()
                    End Try
                End Using
            End Using
        End Function

        Public Function Merge(items As List(Of Address)) As Boolean Implements IAddressRepository.Merge
            Dim mergeTable = New List(Of Object())()

            For Each item In items
                mergeTable.Add(New Object() _
                                  {item.Id, item.AnotherId, item.PersonId, item.DirtyColumns.Contains("PersonId"),
                                   item.Line1, item.DirtyColumns.Contains("Line1"), item.Line2,
                                   item.DirtyColumns.Contains("Line2"), item.Line3, item.DirtyColumns.Contains("Line3"),
                                   item.Line4, item.DirtyColumns.Contains("Line4"), item.PostCode,
                                   item.DirtyColumns.Contains("PostCode"), item.PhoneNumber,
                                   item.DirtyColumns.Contains("PhoneNumber"), item.COUNTRY_CODE,
                                   item.DirtyColumns.Contains("COUNTRY_CODE")})
            Next

            Return BaseMerge(mergeTable)
        End Function

        Protected Overrides Function ToItem(row As DataRow) As Address
            Dim item = New Address With {
                    .Id = GetInt32(row, "Id"),
                    .AnotherId = GetString(row, "AnotherId"),
                    .PersonId = GetInt32(row, "PersonId"),
                    .Line1 = GetString(row, "Line1"),
                    .Line2 = GetString(row, "Line2"),
                    .Line3 = GetString(row, "Line3"),
                    .Line4 = GetString(row, "Line4"),
                    .PostCode = GetString(row, "PostCode"),
                    .PhoneNumber = GetString(row, "PhoneNumber"),
                    .COUNTRY_CODE = GetString(row, "COUNTRY_CODE")
                    }
            item.ResetDirty()
            Return item
        End Function

        Public Function Search(Optional id As Int32? = Nothing, Optional anotherId As String = Nothing,
                               Optional personId As Int32? = Nothing, Optional line1 As String = Nothing,
                               Optional line2 As String = Nothing, Optional line3 As String = Nothing,
                               Optional line4 As String = Nothing, Optional postCode As String = Nothing,
                               Optional phoneNumber As String = Nothing,
                               Optional cOUNTRY_CODE As String = Nothing) As IEnumerable(Of Address) _
            Implements IAddressRepository.Search
            Dim queries = New List(Of QueryItem)()
            If id.HasValue Then queries.Add(New QueryItem("Id", id))
            If Not String.IsNullOrEmpty(anotherId) Then queries.Add(New QueryItem("AnotherId", anotherId))
            If personId.HasValue Then queries.Add(New QueryItem("PersonId", personId))
            If Not String.IsNullOrEmpty(line1) Then queries.Add(New QueryItem("Line1", line1))
            If Not String.IsNullOrEmpty(line2) Then queries.Add(New QueryItem("Line2", line2))
            If Not String.IsNullOrEmpty(line3) Then queries.Add(New QueryItem("Line3", line3))
            If Not String.IsNullOrEmpty(line4) Then queries.Add(New QueryItem("Line4", line4))
            If Not String.IsNullOrEmpty(postCode) Then queries.Add(New QueryItem("PostCode", postCode))
            If Not String.IsNullOrEmpty(phoneNumber) Then queries.Add(New QueryItem("PhoneNumber", phoneNumber))
            If Not String.IsNullOrEmpty(cOUNTRY_CODE) Then queries.Add(New QueryItem("COUNTRY_CODE", cOUNTRY_CODE))
            Return BaseSearch(queries)
        End Function

        Public Function FindById(id As Int32) As IEnumerable(Of Address) Implements IAddressRepository.FindById
            Return FindById(FindComparison.Equals, id)
        End Function

        Public Function FindById(comparison As FindComparison, id As Int32) As IEnumerable(Of Address) Implements IAddressRepository.FindById
            Return _
                Where("Id", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), id).Results()
        End Function

        Public Function FindByAnotherId(anotherId As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByAnotherId
            Return FindByAnotherId(FindComparison.Equals, anotherId)
        End Function

        Public Function FindByAnotherId(comparison As FindComparison, anotherId As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByAnotherId
            Return _
                Where("AnotherId", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison),
                      anotherId).Results()
        End Function

        Public Function FindByPersonId(personId As Int32) As IEnumerable(Of Address) Implements IAddressRepository.FindByPersonId
            Return FindByPersonId(FindComparison.Equals, personId)
        End Function

        Public Function FindByPersonId(comparison As FindComparison, personId As Int32) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByPersonId
            Return _
                Where("PersonId", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), personId) _
                    .Results()
        End Function

        Public Function FindByLine1(line1 As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByLine1
            Return FindByLine1(FindComparison.Equals, line1)
        End Function

        Public Function FindByLine1(comparison As FindComparison, line1 As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByLine1
            Return _
                Where("Line1", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), line1).
                    Results()
        End Function

        Public Function FindByLine2(line2 As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByLine2
            Return FindByLine2(FindComparison.Equals, line2)
        End Function

        Public Function FindByLine2(comparison As FindComparison, line2 As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByLine2
            Return _
                Where("Line2", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), line2).
                    Results()
        End Function

        Public Function FindByLine3(line3 As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByLine3
            Return FindByLine3(FindComparison.Equals, line3)
        End Function

        Public Function FindByLine3(comparison As FindComparison, line3 As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByLine3
            Return _
                Where("Line3", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), line3).
                    Results()
        End Function

        Public Function FindByLine4(line4 As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByLine4
            Return FindByLine4(FindComparison.Equals, line4)
        End Function

        Public Function FindByLine4(comparison As FindComparison, line4 As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByLine4
            Return _
                Where("Line4", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), line4).
                    Results()
        End Function

        Public Function FindByPostCode(postCode As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByPostCode
            Return FindByPostCode(FindComparison.Equals, postCode)
        End Function

        Public Function FindByPostCode(comparison As FindComparison, postCode As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByPostCode
            Return _
                Where("PostCode", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison), postCode) _
                    .Results()
        End Function

        Public Function FindByPhoneNumber(phoneNumber As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByPhoneNumber
            Return FindByPhoneNumber(FindComparison.Equals, phoneNumber)
        End Function

        Public Function FindByPhoneNumber(comparison As FindComparison, phoneNumber As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByPhoneNumber
            Return _
                Where("PhoneNumber", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison),
                      phoneNumber).Results()
        End Function

        Public Function FindByCOUNTRY_CODE(cOUNTRY_CODE As String) As IEnumerable(Of Address) Implements IAddressRepository.FindByCOUNTRY_CODE
            Return FindByCOUNTRY_CODE(FindComparison.Equals, cOUNTRY_CODE)
        End Function

        Public Function FindByCOUNTRY_CODE(comparison As FindComparison, cOUNTRY_CODE As String) _
            As IEnumerable(Of Address) Implements IAddressRepository.FindByCOUNTRY_CODE
            Return _
                Where("COUNTRY_CODE", CType([Enum].Parse(GetType(Comparison), comparison.ToString()), Comparison),
                      cOUNTRY_CODE).Results()
        End Function
    End Class
End Namespace
