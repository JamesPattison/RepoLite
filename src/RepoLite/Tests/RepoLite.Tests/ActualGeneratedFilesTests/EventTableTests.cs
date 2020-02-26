using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;
using System.Linq;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class EventTableTests : BaseTests
    {
        private IEventRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            Data.DropAndCreateDatabase();
            _repository = new EventRepository(ConnectionString);
        }

        [TestMethod]
        public void TestCsvImport()
        {
            var csvPath =
                @"ActualGeneratedFilesTests\Csvs\Event.csv";
            Assert.IsTrue(_repository.Merge(csvPath));

            var item = _repository.Get("EVT_01");

            Assert.IsTrue(item.EventName == "CSV Imported");

            var items = _repository.GetAll();

            Assert.IsTrue(items.Count(x => x.EventName == "CSV Imported") == 1);
        }
    }
}
