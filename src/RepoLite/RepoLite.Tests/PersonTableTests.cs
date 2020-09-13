using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;
using System.Linq;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class PersonTableTests : BaseTests
    {
        private IPersonRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            Data.DropAndCreateDatabase();
            _repository = new PersonRepository(ConnectionString);
        }

        [TestMethod]
        public void TestGet()
        {
            var person = _repository.Get(1);
        }

        [TestMethod]
        public void TestCsvImport()
        {
            var csvPath =
                @"Csvs\Person.csv";
            Assert.IsTrue(_repository.Merge(csvPath));

            var items = _repository.GetAll();

            Assert.IsTrue(items.All(x => x.Nationality == "CSV"));
        }
    }
}
