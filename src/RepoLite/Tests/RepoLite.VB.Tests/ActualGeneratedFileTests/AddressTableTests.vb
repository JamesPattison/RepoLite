Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports RepoLite.VB.Tests.ActualGeneratedFileTests.Base
Imports RepoLite.VB.Tests.NS
Imports RepoLite.VB.Tests.NS.Models
Imports RepoLite.VB.Tests.REPOSITORYNAMESPACE.Base

Namespace ActualGeneratedFileTests
    <TestClass>
    Public Class AddressTableTests
        Inherits BaseTests

        Private _repository As IAddressRepository

        Private Function GetFromDb(addressNumber As Integer) As Address
            Dim address = _repository.GetAll().ToArray()(addressNumber - 1)
            Return If(address, New Address())
        End Function

        Private Sub IsAddress(ByVal retrieved As Address, ByVal addressId As Integer)
            If retrieved Is Nothing Then Throw New ArgumentNullException(NameOf(retrieved))
            Dim addresses = Data.Addresses
            Dim address = addresses.FirstOrDefault(Function(x) x.Id = addressId)
            If address Is Nothing Then Assert.Fail($"Valid address ID range is 1-10, and you entered {addressId}")
            Assert.IsTrue(address.AnotherId = retrieved.AnotherId, $"AnotherId differs-> Expected:{address.AnotherId}, Actual {retrieved.AnotherId}")
            Assert.IsTrue(address.PersonId = retrieved.PersonId, $"PersonId differs-> Expected:{address.PersonId}, Actual {retrieved.PersonId}")
            Assert.IsTrue(address.Line1 = retrieved.Line1, $"Line1 differs-> Expected:{address.Line1}, Actual {retrieved.Line1}")
            Assert.IsTrue(address.Line2 = retrieved.Line2, $"Line2 differs-> Expected:{address.Line2}, Actual {retrieved.Line2}")
            Assert.IsTrue(address.Line3 = retrieved.Line3, $"Line3 differs-> Expected:{address.Line3}, Actual {retrieved.Line3}")
            Assert.IsTrue(address.Line4 = retrieved.Line4, $"Line4 differs-> Expected:{address.Line4}, Actual {retrieved.Line4}")
            Assert.IsTrue(address.PostCode = retrieved.PostCode, $"PostCode differs-> Expected:{address.PostCode}, Actual {retrieved.PostCode}")
            Assert.IsTrue(address.PhoneNumber = retrieved.PhoneNumber, $"PhoneNumber differs-> Expected:{address.PhoneNumber}, Actual {retrieved.PhoneNumber}")
            Assert.IsTrue(address.COUNTRY_CODE = retrieved.COUNTRY_CODE, $"COUNTRY_CODE differs-> Expected:{address.COUNTRY_CODE}, Actual {retrieved.COUNTRY_CODE}")
        End Sub

        <TestInitialize>
        Public Sub TestInitialize()
            Data.DropAndCreateDatabase()
            _repository = New AddressRepository(ConnectionString)
        End Sub

        <TestMethod>
        Public Sub TestGetAll()
            Dim all = _repository.GetAll()
            Assert.IsTrue(all.Count() = 10)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr1()
            Dim retrieved = _repository.[Get](New AddressKeys(1, "77"))
            IsAddress(retrieved, 1)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr2()
            Dim retrieved = _repository.[Get](New AddressKeys(2, "3"))
            IsAddress(retrieved, 2)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr3()
            Dim retrieved = _repository.[Get](New AddressKeys(3, "14"))
            IsAddress(retrieved, 3)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr4()
            Dim retrieved = _repository.[Get](New AddressKeys(4, "2"))
            IsAddress(retrieved, 4)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr5()
            Dim retrieved = _repository.[Get](New AddressKeys(5, "97"))
            IsAddress(retrieved, 5)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr6()
            Dim retrieved = _repository.[Get](New AddressKeys(6, "54"))
            IsAddress(retrieved, 6)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr7()
            Dim retrieved = _repository.[Get](New AddressKeys(7, "1"))
            IsAddress(retrieved, 7)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr8()
            Dim retrieved = _repository.[Get](New AddressKeys(8, "12"))
            IsAddress(retrieved, 8)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr9()
            Dim retrieved = _repository.[Get](New AddressKeys(9, "47"))
            IsAddress(retrieved, 9)
        End Sub

        <TestMethod>
        Public Sub TestGetAddr10()
            Dim retrieved = _repository.[Get](New AddressKeys(10, "47"))
            IsAddress(retrieved, 10)
        End Sub

        <TestMethod>
        Public Sub TestGetMultiAddr1()
            Dim address0 = GetFromDb(1)
            Dim address1 = GetFromDb(1)
            Dim retrieved = _repository.[Get](New AddressKeys(address0.Id, address0.AnotherId), New AddressKeys(address1.Id, address1.AnotherId)).ToArray()
            Assert.IsTrue(retrieved.Length = 2)
            Assert.IsTrue(address0.AnotherId = retrieved(0).AnotherId, $"AnotherId differs-> Expected:{address0.AnotherId}, Actual {retrieved(0).AnotherId}")
            Assert.IsTrue(address0.PersonId = retrieved(0).PersonId, $"PersonId differs-> Expected:{address0.PersonId}, Actual {retrieved(0).PersonId}")
            Assert.IsTrue(address0.Line1 = retrieved(0).Line1, $"Line1 differs-> Expected:{address0.Line1}, Actual {retrieved(0).Line1}")
            Assert.IsTrue(address0.Line2 = retrieved(0).Line2, $"Line2 differs-> Expected:{address0.Line2}, Actual {retrieved(0).Line2}")
            Assert.IsTrue(address0.Line3 = retrieved(0).Line3, $"Line3 differs-> Expected:{address0.Line3}, Actual {retrieved(0).Line3}")
            Assert.IsTrue(address0.Line4 = retrieved(0).Line4, $"Line4 differs-> Expected:{address0.Line4}, Actual {retrieved(0).Line4}")
            Assert.IsTrue(address0.PostCode = retrieved(0).PostCode, $"PostCode differs-> Expected:{address0.PostCode}, Actual {retrieved(0).PostCode}")
            Assert.IsTrue(address0.PhoneNumber = retrieved(0).PhoneNumber, $"PhoneNumber differs-> Expected:{address0.PhoneNumber}, Actual {retrieved(0).PhoneNumber}")
            Assert.IsTrue(address0.COUNTRY_CODE = retrieved(0).COUNTRY_CODE, $"COUNTRY_CODE differs-> Expected:{address0.COUNTRY_CODE}, Actual {retrieved(0).COUNTRY_CODE}")
            Assert.IsTrue(address1.AnotherId = retrieved(1).AnotherId, $"AnotherId differs-> Expected:{address1.AnotherId}, Actual {retrieved(1).AnotherId}")
            Assert.IsTrue(address1.PersonId = retrieved(1).PersonId, $"PersonId differs-> Expected:{address1.PersonId}, Actual {retrieved(1).PersonId}")
            Assert.IsTrue(address1.Line1 = retrieved(1).Line1, $"Line1 differs-> Expected:{address1.Line1}, Actual {retrieved(1).Line1}")
            Assert.IsTrue(address1.Line2 = retrieved(1).Line2, $"Line2 differs-> Expected:{address1.Line2}, Actual {retrieved(1).Line2}")
            Assert.IsTrue(address1.Line3 = retrieved(1).Line3, $"Line3 differs-> Expected:{address1.Line3}, Actual {retrieved(1).Line3}")
            Assert.IsTrue(address1.Line4 = retrieved(1).Line4, $"Line4 differs-> Expected:{address1.Line4}, Actual {retrieved(1).Line4}")
            Assert.IsTrue(address1.PostCode = retrieved(1).PostCode, $"PostCode differs-> Expected:{address1.PostCode}, Actual {retrieved(1).PostCode}")
            Assert.IsTrue(address1.PhoneNumber = retrieved(1).PhoneNumber, $"PhoneNumber differs-> Expected:{address1.PhoneNumber}, Actual {retrieved(1).PhoneNumber}")
            Assert.IsTrue(address1.COUNTRY_CODE = retrieved(1).COUNTRY_CODE, $"COUNTRY_CODE differs-> Expected:{address1.COUNTRY_CODE}, Actual {retrieved(1).COUNTRY_CODE}")
        End Sub

        <TestMethod>
        Public Sub TestGet_ByVals()
            Dim retrieved = _repository.[Get](4, "2")
            IsAddress(retrieved, 4)
        End Sub

        <TestMethod>
        Public Sub TestAddress_Create()
            Dim address = New Address With {
                .AnotherId = "Another ID",
                .PersonId = 1,
                .Line1 = "Address Line 1",
                .PostCode = "Post Code"
            }
            Dim expected = True
            Dim actual = _repository.Create(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Assert.IsTrue(_repository.GetAll().Count() = 11)
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine1()
            Dim address = GetFromDb(1)
            address.Line1 = "712 Newheart Lane"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line1 = "712 Newheart Lane", $"expected: 712 Newheart Lane, but received: {address.Line1}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine2_SetNull()
            Dim address = GetFromDb(1)
            address.Line2 = Nothing
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line2 Is Nothing, $"expected: null, but received: {address.Line2}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine2()
            Dim address = GetFromDb(1)
            address.Line2 = "New Line 2"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line2 = "New Line 2", $"expected: New Line 2, but received: {address.Line2}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine3_SetNull()
            Dim address = GetFromDb(1)
            address.Line3 = Nothing
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line3 Is Nothing, $"expected: null, but received: {address.Line3}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine3()
            Dim address = GetFromDb(1)
            address.Line3 = "New Line 3"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line3 = "New Line 3", $"expected: New Line 3, but received: {address.Line3}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine4_SetNull()
            Dim address = GetFromDb(1)
            address.Line4 = Nothing
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line4 Is Nothing, $"expected: null, but received: {address.Line4}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateLine4()
            Dim address = GetFromDb(1)
            address.Line4 = "New Line 4"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.Line4 = "New Line 4", $"expected: New Line 4, but received: {address.Line4}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdatePostCode_SetNull()
            Dim address = GetFromDb(1)
            Dim prev = address.PostCode
            address.PostCode = Nothing
            Assert.ThrowsException(Of ValidationException)(Function() _repository.Update(address))
            address = GetFromDb(1)
            Assert.IsTrue(address.PostCode = prev, $"expected: {prev}, but received: {address.PostCode}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdatePostCode()
            Dim address = GetFromDb(1)
            address.PostCode = "New Line 4"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.PostCode = "New Line 4", $"expected: New Line 4, but received: {address.PostCode}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdatePhoneNumber_SetNull()
            Dim address = GetFromDb(1)
            address.PhoneNumber = Nothing
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.PhoneNumber Is Nothing, $"expected: null, but received: {address.PhoneNumber}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdatePhoneNumber()
            Dim address = GetFromDb(1)
            address.PhoneNumber = "New Line 4"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.PhoneNumber = "New Line 4", $"expected: New Line 4, but received: {address.PhoneNumber}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateCountryCode_SetNull()
            Dim address = GetFromDb(1)
            address.COUNTRY_CODE = Nothing
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(String.IsNullOrEmpty(address.COUNTRY_CODE), $"expected: null, but received: {address.COUNTRY_CODE}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_UpdateCountryCode()
            Dim address = GetFromDb(1)
            address.COUNTRY_CODE = "GB"
            Dim expected = True
            Dim actual = _repository.Update(address)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            address = GetFromDb(1)
            Assert.IsTrue(address.COUNTRY_CODE = "GB", $"expected: GB, but received: {address.COUNTRY_CODE}")
        End Sub

        '        <TestMethod>
        '        Public Sub TestAddress_CreatePerson_ThenAddresses()
        '            Dim person = New Person With {
        '                .Name = "New Person",
        '                .Nationality = "Irish"
        '            }
        '            Dim actual = New PersonRepository(ConnectionString).Create(person)
        '            Dim expected = True
        '            Assert.IsTrue(actual = expected, $"Create Person: expected: {expected}, but received: {actual}")
        '            Dim address1 = New Address With {
        '                .AnotherId = "Another ID",
        '                .PersonId = person.Id,
        '                .Line1 = "Line 1",
        '                .PostCode = "Post Code"
        '            }
        '            Dim address2 = New Address With {
        '                .AnotherId = "Another 2",
        '                .PersonId = person.Id,
        '                .Line1 = "Line 1 2",
        '                .PostCode = "Post Code 2"
        '            }
        '            actual = _repository.BulkCreate(New List(Of Address) From {
        '                address1,
        '                address2
        '            })
        '            Assert.IsTrue(actual = expected, $"Create addresses: expected: {expected}, but received: {actual}")
        '            Dim all = _repository.GetAll().ToArray()
        '            Assert.IsTrue(all.Length = 12, $"expected: 12 but received: {all.Length}")
        '        End Sub
        '
        '        <TestMethod>
        '        Public Sub TestAddress_CreatePerson_ThenAddresses_ByParams()
        '            Dim person = New Person With {
        '                .Name = "New Person",
        '                .Nationality = "Irish"
        '            }
        '            Dim actual = New PersonRepository(ConnectionString).Create(person)
        '            Dim expected = True
        '            Assert.IsTrue(actual = expected, $"Create Person: expected: {expected}, but received: {actual}")
        '            Dim address1 = New Address With {
        '                .AnotherId = "Another ID",
        '                .PersonId = person.Id,
        '                .Line1 = "Line 1",
        '                .PostCode = "Post Code"
        '            }
        '            Dim address2 = New Address With {
        '                .AnotherId = "Another 2",
        '                .PersonId = person.Id,
        '                .Line1 = "Line 1 2",
        '                .PostCode = "Post Code 2"
        '            }
        '            actual = _repository.BulkCreate(address1, address2)
        '            Assert.IsTrue(actual = expected, $"Create addresses: expected: {expected}, but received: {actual}")
        '            Dim all = _repository.GetAll().ToArray()
        '            Assert.IsTrue(all.Length = 12, $"expected: 12 but received: {all.Length}")
        '        End Sub

        <TestMethod>
        Public Sub TestAddress_Delete_1()
            Dim address = GetFromDb(1)
            Dim expected = True
            Dim actual = _repository.Delete(New AddressKeys(address.Id, address.AnotherId))
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Dim all = _repository.GetAll().ToArray()
            Assert.IsTrue(all.Length = 9, $"expected: 9 but received: {all.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Delete_2()
            Dim address = GetFromDb(1)
            Dim expected = True
            Dim actual = _repository.Delete(New AddressKeys(address.Id, address.AnotherId))
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Dim all = _repository.GetAll().ToArray()
            Assert.IsTrue(all.Length = 9, $"expected: 9 but received: {all.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Delete_ByKeys_1()
            Dim address = GetFromDb(1)
            Dim expected = True
            Dim actual = _repository.Delete(address.Id, address.AnotherId)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Dim all = _repository.GetAll().ToArray()
            Assert.IsTrue(all.Length = 9, $"expected: 9 but received: {all.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Delete_ByKeys_2()
            Dim address = GetFromDb(1)
            Dim expected = True
            Dim actual = _repository.Delete(address.Id, address.AnotherId)
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Dim all = _repository.GetAll().ToArray()
            Assert.IsTrue(all.Length = 9, $"expected: 9 but received: {all.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Delete_ByComposite_1()
            Dim address = GetFromDb(1)
            Dim expected = True
            Dim actual = _repository.Delete(New AddressKeys(address.Id, address.AnotherId))
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Dim all = _repository.GetAll().ToArray()
            Assert.IsTrue(all.Length = 9, $"expected: 9 but received: {all.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Delete_ByComposite_2()
            Dim address = GetFromDb(1)
            Dim expected = True
            Dim actual = _repository.Delete(New AddressKeys(address.Id, address.AnotherId))
            Assert.IsTrue(actual = expected, $"expected: {expected}, but received: {actual}")
            Dim all = _repository.GetAll().ToArray()
            Assert.IsTrue(all.Length = 9, $"expected: 9 but received: {all.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ById()
            Dim actual = _repository.Search(id:=0)

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByAnotherId()
            Dim actual = _repository.Search(anotherId:="47").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByPersonId()
            Dim actual = _repository.Search(personId:=7).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByLine1()
            Dim actual = _repository.Search(line1:="1113 Feil Lock").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByLine2_WithUnion()
            Dim actual = _repository.Search(line2:="North Merle").Union(_repository.Search(line2:="Port Thad")).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByLine3()
            Dim actual = _repository.Search(line3:="Bedfordshire").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByLine4()
            Dim actual = _repository.Search(line4:="Monaco").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByPostCode()
            Dim actual = _repository.Search(postCode:="05472-9584").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByPhoneNumber()
            Dim actual = _repository.Search(phoneNumber:="01833 406229").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Search_ByCountryCode()
            Dim actual = _repository.Search(cOUNTRY_CODE:="AE").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindById()
            Dim actual = _repository.FindById(1).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindById_NotEquals()
            Dim actual = _repository.FindById(FindComparison.NotEquals, 7).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindById_Comparison_LessThan()
            Dim actual = _repository.FindById(FindComparison.LessThan, 5).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 4
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindById_Comparison_GreaterThanOrEquals()
            Dim actual = _repository.FindById(FindComparison.GreaterThanOrEquals, 4).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 7
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByAnotherId()
            Dim actual = _repository.FindByAnotherId("77").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByAnotherId_NotEquals()
            Dim actual = _repository.FindByAnotherId(FindComparison.NotEquals, "77").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByAnotherId_Comparison_Like()
            Dim actual = _repository.FindByAnotherId(FindComparison.[Like], "7").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 4
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByAnotherId_Comparison_NotLike()
            Dim actual = _repository.FindByAnotherId(FindComparison.NotLike, "5").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPersonId()
            Dim actual = _repository.FindByPersonId(8).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPersonId_NotLike()
            Dim actual = _repository.FindByPersonId(FindComparison.NotLike, 8).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPersonId_Comparison_LessThan()
            Dim actual = _repository.FindByPersonId(FindComparison.LessThan, 9).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPersonId_Comparison_GreaterThanOrEquals()
            Dim actual = _repository.FindByPersonId(FindComparison.GreaterThanOrEquals, 9).ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine1()
            Dim actual = _repository.FindByLine1("91643 Cormier Bridge").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine1_NotLike()
            Dim actual = _repository.FindByLine1(FindComparison.NotLike, "91643 Cormier Bridge").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine1_Comparison_Like()
            Dim actual = _repository.FindByLine1(FindComparison.[Like], "13").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine1_Comparison_NotLike()
            Dim actual = _repository.FindByLine1(FindComparison.NotLike, "13").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 8
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine2()
            Dim actual = _repository.FindByLine2("North Johnny").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine2_NotLike()
            Dim actual = _repository.FindByLine2(FindComparison.NotLike, "North Johnny").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine2_Comparison_Like()
            Dim actual = _repository.FindByLine2(FindComparison.[Like], "M").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine2_Comparison_NotLike()
            Dim actual = _repository.FindByLine2(FindComparison.NotLike, "T").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine3()
            Dim actual = _repository.FindByLine3("Bedfordshire").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine3_NotLike()
            Dim actual = _repository.FindByLine3(FindComparison.NotLike, "Bedfordshire").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 7
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine3_Comparison_Like()
            Dim actual = _repository.FindByLine3(FindComparison.[Like], "shire").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 6
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine3_Comparison_NotLike()
            Dim actual = _repository.FindByLine3(FindComparison.NotLike, "shire").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 4
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine4()
            Dim actual = _repository.FindByLine4("Monaco").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine4_NotLike()
            Dim actual = _repository.FindByLine4(FindComparison.NotLike, "United States Minor Outlying Islands").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine4_Comparison_Like()
            Dim actual = _repository.FindByLine4(FindComparison.[Like], "a").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 10
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByLine4_Comparison_NotLike()
            Dim actual = _repository.FindByLine4(FindComparison.NotLike, "n").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPostCode()
            Dim actual = _repository.FindByPostCode("10326-2100").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPostCode_NotLike()
            Dim actual = _repository.FindByPostCode(FindComparison.NotLike, "05472-9584").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPostCode_Comparison_Like()
            Dim actual = _repository.FindByPostCode(FindComparison.[Like], "25").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPostCode_Comparison_NotLike()
            Dim actual = _repository.FindByPostCode(FindComparison.NotLike, "8").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 5
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPhoneNumber()
            Dim actual = _repository.FindByPhoneNumber("01728 542265").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPhoneNumber_NotLike()
            Dim actual = _repository.FindByPhoneNumber(FindComparison.NotLike, "01833 406229").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPhoneNumber_Comparison_Like()
            Dim actual = _repository.FindByPhoneNumber(FindComparison.[Like], "18").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByPhoneNumber_Comparison_NotLike()
            Dim actual = _repository.FindByPhoneNumber(FindComparison.NotLike, "7").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 5
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByCountryCode()
            Dim actual = _repository.FindByCOUNTRY_CODE("TW").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 1
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByCountryCode_NotLike()
            Dim actual = _repository.FindByCOUNTRY_CODE(FindComparison.NotLike, "TW").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByCountryCode_Comparison_Like()
            Dim actual = _repository.FindByCOUNTRY_CODE(FindComparison.[Like], "m").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_FindByCountryCode_Comparison_NotLike()
            Dim actual = _repository.FindByCOUNTRY_CODE(FindComparison.NotLike, "c").ToArray()

            For Each addr In actual
                IsAddress(addr, addr.Id)
            Next

            Dim expected = 8
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Merge()
            Dim addresses = New List(Of Address)()

            For i = 0 To 20 - 1
                Dim address = New Address With {
                    .Id = i + 1
                }

                If i < 10 Then
                    address.AnotherId = Data.Addresses(i).AnotherId
                    address.Line1 = $"MERGED LINE 1_{i + 1}"
                    address.Line2 = $"MERGED LINE 2_{i + 1}"
                Else
                    address.AnotherId = (i + 1).ToString()
                    address.PersonId = i Mod 5 + 1
                    address.Line1 = $"LINE 1_{i + 1}"
                    address.PostCode = $"POSTCODE 1_{i + 1}"
                End If

                addresses.Add(address)
            Next

            _repository.Merge(addresses)
            Dim retrieved = _repository.GetAll().ToArray()
            Assert.IsTrue(retrieved.Count() = 20, $"expected: {20} but received: {retrieved.Count()}")
            Dim address0 = _repository.[Get](1, "77")
            Assert.IsTrue(address0.AnotherId = Data.Addresses(0).AnotherId, $"AnotherId differs-> Expected:{address0.AnotherId}, Actual {Data.Addresses(0).AnotherId}")
            Assert.IsTrue(address0.PersonId = Data.Addresses(0).PersonId, $"PersonId differs-> Expected:{address0.PersonId}, Actual {Data.Addresses(0).PersonId}")
            Assert.IsTrue(address0.Line1 = "MERGED LINE 1_1", $"Line1 differs-> Expected:{address0.Line1}, Actual MERGED LINE 1_1")
            Assert.IsTrue(address0.Line2 = "MERGED LINE 2_1", $"Line2 differs-> Expected:{address0.Line2}, Actual MERGED LINE 2_1")
            Assert.IsTrue(address0.Line3 = Data.Addresses(0).Line3, $"Line3 differs-> Expected:{address0.Line3}, Actual {Data.Addresses(0).Line3}")
            Assert.IsTrue(address0.Line4 = Data.Addresses(0).Line4, $"Line4 differs-> Expected:{address0.Line4}, Actual {Data.Addresses(0).Line4}")
            Assert.IsTrue(address0.PostCode = Data.Addresses(0).PostCode, $"PostCode differs-> Expected:{address0.PostCode}, Actual {Data.Addresses(0).PostCode}")
            Assert.IsTrue(address0.PhoneNumber = Data.Addresses(0).PhoneNumber, $"PhoneNumber differs-> Expected:{address0.PhoneNumber}, Actual {Data.Addresses(0).PhoneNumber}")
            Assert.IsTrue(address0.COUNTRY_CODE = Data.Addresses(0).COUNTRY_CODE, $"COUNTRY_CODE differs-> Expected:{address0.COUNTRY_CODE}, Actual {Data.Addresses(0).COUNTRY_CODE}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Where_1()
            Dim actual = _repository.Where(NameOf(Address.Line1), Comparison.[Like], "Co").Results().ToArray()
            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Where_2()
            Dim actual = _repository.Where(NameOf(Address.PersonId), Comparison.GreaterThanOrEquals, 4).[And](NameOf(Address.Id), Comparison.LessThan, 4).Results().ToArray()
            Dim expected = 2
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Where_3()
            Dim actual = _repository.Where(NameOf(Address.PersonId), Comparison.GreaterThanOrEquals, 4).[Or](NameOf(Address.Id), Comparison.LessThan, 4).Results().ToArray()
            Dim expected = 9
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Where_4()
            Dim actual = _repository.Where(NameOf(Address.Line4), Comparison.Equals, "Monaco").OrBeginGroup(NameOf(Address.AnotherId), Comparison.[Like], "47").[And](NameOf(Address.PersonId), Comparison.LessThan, 7).EndGroup().Results().ToArray()
            Dim expected = 3
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub

        <TestMethod>
        Public Sub TestAddress_Where_5()
            Dim actual = _repository.Where("[Id] > 6").ToArray()
            Dim expected = 4
            Assert.IsTrue(actual.Length = expected, $"expected: {expected} but received: {actual.Length}")
        End Sub
    End Class
End Namespace
