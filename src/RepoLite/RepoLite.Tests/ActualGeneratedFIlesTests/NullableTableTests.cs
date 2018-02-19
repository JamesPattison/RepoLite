using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using NS.Models;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;
using System;
using System.Linq;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class NullableTableTests : BaseTests
    {
        private INullableTableRepository _repository;

        private NullableTable GetFromDb(int nullableId)
        {
            var nullable = _repository.GetAll().ToArray()[nullableId - 1];
            return nullable ?? new NullableTable();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new NullableTableRepository(ConnectionString);
            Data.DropAndCreateDatabase();
        }

        [TestMethod]
        public void TestGetAll()
        {
            var all = _repository.GetAll();

            Assert.IsTrue(all.Count() == 3);
        }

        [TestMethod]
        public void TestNullable_Create()
        {
            var nullable = new NullableTable();

            var expected = true;
            var actual = _repository.Create(nullable);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            Assert.IsTrue(_repository.GetAll().Count() == 4);
        }

        [TestMethod]
        public void TestNullable_Create_NotNull()
        {
            var nullable = new NullableTable
            {
                Age = 1,
                DoB = DateTime.Now,
                LolVal = Guid.NewGuid()
            };

            var expected = true;
            var actual = _repository.Create(nullable);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            Assert.IsTrue(_repository.GetAll().Count() == 4);
        }

        [TestMethod]
        public void TestAddress_FindByAge()
        {
            var actual = _repository.FindByAge(31).ToArray();

            var expected = 1;
            Assert.IsTrue(actual.Length == expected, $"expected: {expected} but received: {actual.Length}");
        }
    }
}
