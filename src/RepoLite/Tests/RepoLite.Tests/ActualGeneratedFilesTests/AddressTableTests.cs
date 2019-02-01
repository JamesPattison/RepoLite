using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using NS.Base;
using NS.Models;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class AddressTableTests : BaseTests
    {
        private IAddressRepository _repository;

        private Address GetFromDb(int addressNumber)
        {
            var address = _repository.GetAll().ToArray()[addressNumber - 1];
            return address ?? new Address();
        }

        private void IsAddress(Address retrieved, int addressId)
        {
            if (retrieved == null)
                throw new ArgumentNullException(nameof(retrieved));
            var addresses = Data.Addresses;
            var address = addresses.FirstOrDefault(x => x.Id == addressId);
            if (address == null)
                Assert.Fail($"Valid address ID range is 1-10, and you entered {addressId}");

            Assert.IsTrue(address.AnotherId == retrieved.AnotherId,
                $"AnotherId differs-> Expected:{address.AnotherId}, Actual {retrieved.AnotherId}");
            Assert.IsTrue(address.PersonId == retrieved.PersonId,
                $"PersonId differs-> Expected:{address.PersonId}, Actual {retrieved.PersonId}");
            Assert.IsTrue(address.Line1 == retrieved.Line1,
                $"Line1 differs-> Expected:{address.Line1}, Actual {retrieved.Line1}");
            Assert.IsTrue(address.Line2 == retrieved.Line2,
                $"Line2 differs-> Expected:{address.Line2}, Actual {retrieved.Line2}");
            Assert.IsTrue(address.Line3 == retrieved.Line3,
                $"Line3 differs-> Expected:{address.Line3}, Actual {retrieved.Line3}");
            Assert.IsTrue(address.Line4 == retrieved.Line4,
                $"Line4 differs-> Expected:{address.Line4}, Actual {retrieved.Line4}");
            Assert.IsTrue(address.PostCode == retrieved.PostCode,
                $"PostCode differs-> Expected:{address.PostCode}, Actual {retrieved.PostCode}");
            Assert.IsTrue(address.PhoneNumber == retrieved.PhoneNumber,
                $"PhoneNumber differs-> Expected:{address.PhoneNumber}, Actual {retrieved.PhoneNumber}");
            Assert.IsTrue(address.COUNTRY_CODE == retrieved.COUNTRY_CODE,
                $"COUNTRY_CODE differs-> Expected:{address.COUNTRY_CODE}, Actual {retrieved.COUNTRY_CODE}");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Data.DropAndCreateDatabase();
            _repository = new AddressRepository(ConnectionString);
        }

        [TestMethod]
        public void TestGetAll()
        {
            var all = _repository.GetAll();

            Assert.IsTrue(all.Count() == 10);
        }

        [TestMethod]
        public void TestRecordCount()
        {
            var count = _repository.RecordCount();

            Assert.IsTrue(count == 10);
        }

        [TestMethod]
        public void TestGetAddr1()
        {
            var retrieved = _repository.Get(new AddressKeys(1, "77"));
            IsAddress(retrieved, 1);
        }

        [TestMethod]
        public void TestGetAddr2()
        {
            var retrieved = _repository.Get(new AddressKeys(2, "3"));
            IsAddress(retrieved, 2);
        }

        [TestMethod]
        public void TestGetAddr3()
        {
            var retrieved = _repository.Get(new AddressKeys(3, "14"));
            IsAddress(retrieved, 3);
        }

        [TestMethod]
        public void TestGetAddr4()
        {
            var retrieved = _repository.Get(new AddressKeys(4, "2"));
            IsAddress(retrieved, 4);
        }

        [TestMethod]
        public void TestGetAddr5()
        {
            var retrieved = _repository.Get(new AddressKeys(5, "97"));
            IsAddress(retrieved, 5);
        }

        [TestMethod]
        public void TestGetAddr6()
        {
            var retrieved = _repository.Get(new AddressKeys(6, "54"));
            IsAddress(retrieved, 6);
        }

        [TestMethod]
        public void TestGetAddr7()
        {
            var retrieved = _repository.Get(new AddressKeys(7, "1"));
            IsAddress(retrieved, 7);
        }

        [TestMethod]
        public void TestGetAddr8()
        {
            var retrieved = _repository.Get(new AddressKeys(8, "12"));
            IsAddress(retrieved, 8);
        }

        [TestMethod]
        public void TestGetAddr9()
        {
            var retrieved = _repository.Get(new AddressKeys(9, "47"));
            IsAddress(retrieved, 9);
        }

        [TestMethod]
        public void TestGetAddr10()
        {
            var retrieved = _repository.Get(new AddressKeys(10, "47"));
            IsAddress(retrieved, 10);
        }

        [TestMethod]
        public void TestGetMultiAddr1()
        {
            var address0 = GetFromDb(1);
            var address1 = GetFromDb(1);

            var retrieved = _repository.Get(new AddressKeys(address0.Id, address0.AnotherId), new AddressKeys(address1.Id, address1.AnotherId)).ToArray();

            Assert.IsTrue(retrieved.Length == 2);

            Assert.IsTrue(address0.AnotherId == retrieved[0].AnotherId, $"AnotherId differs-> Expected:{address0.AnotherId}, Actual {retrieved[0].AnotherId}");
            Assert.IsTrue(address0.PersonId == retrieved[0].PersonId, $"PersonId differs-> Expected:{address0.PersonId}, Actual {retrieved[0].PersonId}");
            Assert.IsTrue(address0.Line1 == retrieved[0].Line1, $"Line1 differs-> Expected:{address0.Line1}, Actual {retrieved[0].Line1}");
            Assert.IsTrue(address0.Line2 == retrieved[0].Line2, $"Line2 differs-> Expected:{address0.Line2}, Actual {retrieved[0].Line2}");
            Assert.IsTrue(address0.Line3 == retrieved[0].Line3, $"Line3 differs-> Expected:{address0.Line3}, Actual {retrieved[0].Line3}");
            Assert.IsTrue(address0.Line4 == retrieved[0].Line4, $"Line4 differs-> Expected:{address0.Line4}, Actual {retrieved[0].Line4}");
            Assert.IsTrue(address0.PostCode == retrieved[0].PostCode, $"PostCode differs-> Expected:{address0.PostCode}, Actual {retrieved[0].PostCode}");
            Assert.IsTrue(address0.PhoneNumber == retrieved[0].PhoneNumber, $"PhoneNumber differs-> Expected:{address0.PhoneNumber}, Actual {retrieved[0].PhoneNumber}");
            Assert.IsTrue(address0.COUNTRY_CODE == retrieved[0].COUNTRY_CODE, $"COUNTRY_CODE differs-> Expected:{address0.COUNTRY_CODE}, Actual {retrieved[0].COUNTRY_CODE}");

            Assert.IsTrue(address1.AnotherId == retrieved[1].AnotherId, $"AnotherId differs-> Expected:{address1.AnotherId}, Actual {retrieved[1].AnotherId}");
            Assert.IsTrue(address1.PersonId == retrieved[1].PersonId, $"PersonId differs-> Expected:{address1.PersonId}, Actual {retrieved[1].PersonId}");
            Assert.IsTrue(address1.Line1 == retrieved[1].Line1, $"Line1 differs-> Expected:{address1.Line1}, Actual {retrieved[1].Line1}");
            Assert.IsTrue(address1.Line2 == retrieved[1].Line2, $"Line2 differs-> Expected:{address1.Line2}, Actual {retrieved[1].Line2}");
            Assert.IsTrue(address1.Line3 == retrieved[1].Line3, $"Line3 differs-> Expected:{address1.Line3}, Actual {retrieved[1].Line3}");
            Assert.IsTrue(address1.Line4 == retrieved[1].Line4, $"Line4 differs-> Expected:{address1.Line4}, Actual {retrieved[1].Line4}");
            Assert.IsTrue(address1.PostCode == retrieved[1].PostCode, $"PostCode differs-> Expected:{address1.PostCode}, Actual {retrieved[1].PostCode}");
            Assert.IsTrue(address1.PhoneNumber == retrieved[1].PhoneNumber, $"PhoneNumber differs-> Expected:{address1.PhoneNumber}, Actual {retrieved[1].PhoneNumber}");
            Assert.IsTrue(address1.COUNTRY_CODE == retrieved[1].COUNTRY_CODE, $"COUNTRY_CODE differs-> Expected:{address1.COUNTRY_CODE}, Actual {retrieved[1].COUNTRY_CODE}");
        }

        [TestMethod]
        public void TestGet_ByVals()
        {
            var retrieved = _repository.Get(4, "2");
            IsAddress(retrieved, 4);
        }

        [TestMethod]
        public void TestAddress_Create()
        {
            var address = new Address
            {
                AnotherId = "Another ID",
                PersonId = 1,
                Line1 = "Address Line 1",
                PostCode = "Post Code"
            };

            var expected = true;
            var actual = _repository.Create(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            Assert.IsTrue(_repository.GetAll().Count() == 11);
        }

        [TestMethod]
        public void TestAddress_UpdateLine1()
        {
            var address = GetFromDb(1);

            address.Line1 = "712 Newheart Lane";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line1 == "712 Newheart Lane", $"expected: 712 Newheart Lane, but received: {address.Line1 }");
        }

        [TestMethod]
        public void TestAddress_UpdateLine2_SetNull()
        {
            var address = GetFromDb(1);

            address.Line2 = null;

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line2 == null, $"expected: null, but received: {address.Line2}");
        }

        [TestMethod]
        public void TestAddress_UpdateLine2()
        {
            var address = GetFromDb(1);

            address.Line2 = "New Line 2";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line2 == "New Line 2", $"expected: New Line 2, but received: {address.Line2}");
        }

        [TestMethod]
        public void TestAddress_UpdateLine3_SetNull()
        {
            var address = GetFromDb(1);

            address.Line3 = null;

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line3 == null, $"expected: null, but received: {address.Line3}");
        }


        [TestMethod]
        public void TestAddress_UpdateLine3()
        {
            var address = GetFromDb(1);

            address.Line3 = "New Line 3";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line3 == "New Line 3", $"expected: New Line 3, but received: {address.Line3}");
        }

        [TestMethod]
        public void TestAddress_UpdateLine4_SetNull()
        {
            var address = GetFromDb(1);

            address.Line4 = null;

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line4 == null, $"expected: null, but received: {address.Line4}");
        }
        [TestMethod]
        public void TestAddress_UpdateLine4()
        {
            var address = GetFromDb(1);

            address.Line4 = "New Line 4";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.Line4 == "New Line 4", $"expected: New Line 4, but received: {address.Line4}");
        }

        [TestMethod]
        public void TestAddress_UpdatePostCode_SetNull()
        {
            var address = GetFromDb(1);

            var prev = address.PostCode;
            address.PostCode = null;

            Assert.ThrowsException<ValidationException>(() => _repository.Update(address));

            address = GetFromDb(1);
            Assert.IsTrue(address.PostCode == prev, $"expected: {prev}, but received: {address.PostCode}");
        }

        [TestMethod]
        public void TestAddress_UpdatePostCode()
        {
            var address = GetFromDb(1);

            address.PostCode = "New Line 4";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.PostCode == "New Line 4", $"expected: New Line 4, but received: {address.PostCode}");
        }

        [TestMethod]
        public void TestAddress_UpdatePhoneNumber_SetNull()
        {
            var address = GetFromDb(1);

            address.PhoneNumber = null;

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.PhoneNumber == null, $"expected: null, but received: {address.PhoneNumber}");
        }
        [TestMethod]
        public void TestAddress_UpdatePhoneNumber()
        {
            var address = GetFromDb(1);

            address.PhoneNumber = "New Line 4";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.PhoneNumber == "New Line 4", $"expected: New Line 4, but received: {address.PhoneNumber}");
        }

        [TestMethod]
        public void TestAddress_UpdateCountryCode_SetNull()
        {
            var address = GetFromDb(1);

            address.COUNTRY_CODE = null;

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.COUNTRY_CODE == null, $"expected: null, but received: {address.COUNTRY_CODE}");
        }

        [TestMethod]
        public void TestAddress_UpdateCountryCode()
        {
            var address = GetFromDb(1);

            address.COUNTRY_CODE = "GB";

            var expected = true;
            var actual = _repository.Update(address);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            address = GetFromDb(1);
            Assert.IsTrue(address.COUNTRY_CODE == "GB", $"expected: GB, but received: {address.COUNTRY_CODE}");
        }

        [TestMethod]
        public void TestAddress_CreatePerson_ThenAddresses()
        {
            var person = new Person
            {
                Name = "New Person",
                Nationality = "Irish"
            };

            var actual = new PersonRepository(ConnectionString).Create(person);
            var expected = true;

            Assert.IsTrue(actual == expected, $"Create Person: expected: {expected}, but received: {actual}");

            var address1 = new Address
            {
                AnotherId = "Another ID",
                PersonId = person.Id,
                Line1 = "Line 1",
                PostCode = "Post Code"
            };
            var address2 = new Address
            {
                AnotherId = "Another 2",
                PersonId = person.Id,
                Line1 = "Line 1 2",
                PostCode = "Post Code 2"
            };

            actual = _repository.BulkCreate(new List<Address> { address1, address2 });

            Assert.IsTrue(actual == expected, $"Create addresses: expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 12, $"expected: 12 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_CreatePerson_ThenAddresses_ByParams()
        {
            var person = new Person
            {
                Name = "New Person",
                Nationality = "Irish"
            };

            var actual = new PersonRepository(ConnectionString).Create(person);
            var expected = true;

            Assert.IsTrue(actual == expected, $"Create Person: expected: {expected}, but received: {actual}");

            var address1 = new Address
            {
                AnotherId = "Another ID",
                PersonId = person.Id,
                Line1 = "Line 1",
                PostCode = "Post Code"
            };
            var address2 = new Address
            {
                AnotherId = "Another 2",
                PersonId = person.Id,
                Line1 = "Line 1 2",
                PostCode = "Post Code 2"
            };

            actual = _repository.BulkCreate(address1, address2);

            Assert.IsTrue(actual == expected, $"Create addresses: expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 12, $"expected: 12 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Delete_1()
        {
            var address = GetFromDb(1);

            var expected = true;
            var actual = _repository.Delete(new AddressKeys(address.Id, address.AnotherId));

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 9, $"expected: 9 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Delete_2()
        {
            var address = GetFromDb(1);

            var expected = true;
            var actual = _repository.Delete(new AddressKeys(address.Id, address.AnotherId));

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 9, $"expected: 9 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Delete_ByKeys_1()
        {
            var address = GetFromDb(1);

            var expected = true;
            var actual = _repository.Delete(address.Id, address.AnotherId);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 9, $"expected: 9 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Delete_ByKeys_2()
        {
            var address = GetFromDb(1);

            var expected = true;
            var actual = _repository.Delete(address.Id, address.AnotherId);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 9, $"expected: 9 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Delete_ByComposite_1()
        {
            var address = GetFromDb(1);

            var expected = true;
            var actual = _repository.Delete(new AddressKeys(address.Id, address.AnotherId));

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 9, $"expected: 9 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Delete_ByComposite_2()
        {
            var address = GetFromDb(1);

            var expected = true;
            var actual = _repository.Delete(new AddressKeys(address.Id, address.AnotherId));

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            var all = _repository.GetAll().ToArray();
            Assert.IsTrue(all.Length == 9, $"expected: 9 but received: {all.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ById()
        {
            var actual = _repository.Search(id: 0);

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }
        }

        [TestMethod]
        public void TestAddress_Search_ByAnotherId()
        {
            var actual = _repository.Search(anotherid: "47").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByPersonId()
        {
            var actual = _repository.Search(personid: 7).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByLine1()
        {
            var actual = _repository.Search(line1: "1113 Feil Lock").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByLine2_WithUnion()
        {
            var actual = _repository.Search(line2: "North Merle").Union(_repository.Search(line2: "Port Thad")).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByLine3()
        {
            var actual = _repository.Search(line3: "Bedfordshire").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByLine4()
        {
            var actual = _repository.Search(line4: "Monaco").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByPostCode()
        {
            var actual = _repository.Search(postcode: "05472-9584").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByPhoneNumber()
        {
            var actual = _repository.Search(phonenumber: "01833 406229").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Search_ByCountryCode()
        {
            var actual = _repository.Search(country_code: "AE").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindById()
        {
            var actual = _repository.FindById(1).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindById_NotEquals()
        {
            var actual = _repository.FindById(FindComparison.NotEquals, 7).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindById_Comparison_LessThan()
        {
            var actual = _repository.FindById(FindComparison.LessThan, 5).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 4;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindById_Comparison_GreaterThanOrEquals()
        {
            var actual = _repository.FindById(FindComparison.GreaterThanOrEquals, 4).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 7;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByAnotherId()
        {
            var actual = _repository.FindByAnotherId("77").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByAnotherId_NotEquals()
        {
            var actual = _repository.FindByAnotherId(FindComparison.NotEquals, "77").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByAnotherId_Comparison_Like()
        {
            var actual = _repository.FindByAnotherId(FindComparison.Like, "7").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 4;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByAnotherId_Comparison_NotLike()
        {
            var actual = _repository.FindByAnotherId(FindComparison.NotLike, "5").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPersonId()
        {
            var actual = _repository.FindByPersonId(8).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPersonId_NotLike()
        {
            var actual = _repository.FindByPersonId(FindComparison.NotLike, 8).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPersonId_Comparison_LessThan()
        {
            var actual = _repository.FindByPersonId(FindComparison.LessThan, 9).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPersonId_Comparison_GreaterThanOrEquals()
        {
            var actual = _repository.FindByPersonId(FindComparison.GreaterThanOrEquals, 9).ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine1()
        {
            var actual = _repository.FindByLine1("91643 Cormier Bridge").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine1_NotLike()
        {
            var actual = _repository.FindByLine1(FindComparison.NotLike, "91643 Cormier Bridge").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine1_Comparison_Like()
        {
            var actual = _repository.FindByLine1(FindComparison.Like, "13").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine1_Comparison_NotLike()
        {
            var actual = _repository.FindByLine1(FindComparison.NotLike, "13").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 8;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine2()
        {
            var actual = _repository.FindByLine2("North Johnny").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine2_NotLike()
        {
            var actual = _repository.FindByLine2(FindComparison.NotLike, "North Johnny").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine2_Comparison_Like()
        {
            var actual = _repository.FindByLine2(FindComparison.Like, "M").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine2_Comparison_NotLike()
        {
            var actual = _repository.FindByLine2(FindComparison.NotLike, "T").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine3()
        {
            var actual = _repository.FindByLine3("Bedfordshire").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine3_NotLike()
        {
            var actual = _repository.FindByLine3(FindComparison.NotLike, "Bedfordshire").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 7;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine3_Comparison_Like()
        {
            var actual = _repository.FindByLine3(FindComparison.Like, "shire").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 6;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine3_Comparison_NotLike()
        {
            var actual = _repository.FindByLine3(FindComparison.NotLike, "shire").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 4;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine4()
        {
            var actual = _repository.FindByLine4("Monaco").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine4_NotLike()
        {
            var actual = _repository.FindByLine4(FindComparison.NotLike, "United States Minor Outlying Islands").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine4_Comparison_Like()
        {
            var actual = _repository.FindByLine4(FindComparison.Like, "a").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 10;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByLine4_Comparison_NotLike()
        {
            var actual = _repository.FindByLine4(FindComparison.NotLike, "n").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPostCode()
        {
            var actual = _repository.FindByPostCode("10326-2100").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPostCode_NotLike()
        {
            var actual = _repository.FindByPostCode(FindComparison.NotLike, "05472-9584").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPostCode_Comparison_Like()
        {
            var actual = _repository.FindByPostCode(FindComparison.Like, "25").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPostCode_Comparison_NotLike()
        {
            var actual = _repository.FindByPostCode(FindComparison.NotLike, "8").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 5;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPhoneNumber()
        {
            var actual = _repository.FindByPhoneNumber("01728 542265").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPhoneNumber_NotLike()
        {
            var actual = _repository.FindByPhoneNumber(FindComparison.NotLike, "01833 406229").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPhoneNumber_Comparison_Like()
        {
            var actual = _repository.FindByPhoneNumber(FindComparison.Like, "18").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByPhoneNumber_Comparison_NotLike()
        {
            var actual = _repository.FindByPhoneNumber(FindComparison.NotLike, "7").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 5;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByCountryCode()
        {
            var actual = _repository.FindByCOUNTRY_CODE("TW").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByCountryCode_NotLike()
        {
            var actual = _repository.FindByCOUNTRY_CODE(FindComparison.NotLike, "TW").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByCountryCode_Comparison_Like()
        {
            var actual = _repository.FindByCOUNTRY_CODE(FindComparison.Like, "m").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_FindByCountryCode_Comparison_NotLike()
        {
            var actual = _repository.FindByCOUNTRY_CODE(FindComparison.NotLike, "c").ToArray();

            foreach (var addr in actual)
            {
                IsAddress(addr, addr.Id);
            }

            var expected = 8;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Merge()
        {
            var addresses = new List<Address>();
            //Merge 20 addresses into the 10 existing
            for (var i = 0; i < 20; i++)
            {
                var address = new Address { Id = i + 1 };
                if (i < 10)
                {
                    address.AnotherId = Data.Addresses[i].AnotherId;
                    address.Line1 = $"MERGED LINE 1_{i + 1}";
                    address.Line2 = $"MERGED LINE 2_{i + 1}";
                }
                else
                {
                    address.AnotherId = (i + 1).ToString();
                    address.PersonId = i % 5 + 1;
                    address.Line1 = $"LINE 1_{i + 1}";
                    address.PostCode = $"POSTCODE 1_{i + 1}";
                }

                addresses.Add(address);
            }

            _repository.Merge(addresses);

            var retrieved = _repository.GetAll().ToArray();
            Assert.IsTrue(retrieved.Count() == 20, $"expected: {20} but received: {retrieved.Count()}");

            //Make sure unchanged properties weren't changed in the merge

            var address0 = _repository.Get(1, "77");

            Assert.IsTrue(address0.AnotherId == Data.Addresses[0].AnotherId,
                $"AnotherId differs-> Expected:{address0.AnotherId}, Actual {Data.Addresses[0].AnotherId}");
            Assert.IsTrue(address0.PersonId == Data.Addresses[0].PersonId,
                $"PersonId differs-> Expected:{address0.PersonId}, Actual {Data.Addresses[0].PersonId}");
            Assert.IsTrue(address0.Line1 == "MERGED LINE 1_1",
                $"Line1 differs-> Expected:{address0.Line1}, Actual MERGED LINE 1_1");
            Assert.IsTrue(address0.Line2 == "MERGED LINE 2_1",
                $"Line2 differs-> Expected:{address0.Line2}, Actual MERGED LINE 2_1");
            Assert.IsTrue(address0.Line3 == Data.Addresses[0].Line3,
                $"Line3 differs-> Expected:{address0.Line3}, Actual {Data.Addresses[0].Line3}");
            Assert.IsTrue(address0.Line4 == Data.Addresses[0].Line4,
                $"Line4 differs-> Expected:{address0.Line4}, Actual {Data.Addresses[0].Line4}");
            Assert.IsTrue(address0.PostCode == Data.Addresses[0].PostCode,
                $"PostCode differs-> Expected:{address0.PostCode}, Actual {Data.Addresses[0].PostCode}");
            Assert.IsTrue(address0.PhoneNumber == Data.Addresses[0].PhoneNumber,
                $"PhoneNumber differs-> Expected:{address0.PhoneNumber}, Actual {Data.Addresses[0].PhoneNumber}");
            Assert.IsTrue(address0.COUNTRY_CODE == Data.Addresses[0].COUNTRY_CODE,
                $"COUNTRY_CODE differs-> Expected:{address0.COUNTRY_CODE}, Actual {Data.Addresses[0].COUNTRY_CODE}");
        }

        [TestMethod]
        public void TestAddress_Where_1()
        {
            var actual = _repository.Where(nameof(Address.Line1), Comparison.Like, "Co").Results().ToArray();

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Where_2()
        {
            var actual = _repository.Where(nameof(Address.PersonId), Comparison.GreaterThanOrEquals, 4).And(nameof(Address.Id), Comparison.LessThan, 4).Results().ToArray();

            var expected = 2;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Where_3()
        {
            var actual = _repository.Where(nameof(Address.PersonId), Comparison.GreaterThanOrEquals, 4).Or(nameof(Address.Id), Comparison.LessThan, 4).Results().ToArray();

            var expected = 9;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Where_4()
        {
            var actual = _repository.Where(nameof(Address.Line4), Comparison.Equals, "Monaco")
                .OrBeginGroup(nameof(Address.AnotherId), Comparison.Like, "47")
                .And(nameof(Address.PersonId), Comparison.LessThan, 7)
                .EndGroup().Results().ToArray();

            var expected = 3;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

        [TestMethod]
        public void TestAddress_Where_5()
        {
            var actual = _repository.Where("[Id] > 6").ToArray();

            var expected = 4;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }

    }
}