using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using NS.Models;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;
using System.Linq;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class BIN_MASTERTests : BaseTests
    {
        private IBIN_MASTERRepository _repository;

        private BIN_MASTER GetFromDb(int binNo)
        {
            var bin = _repository.GetAll().ToArray()[binNo - 1];
            return bin ?? new BIN_MASTER();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new BIN_MASTERRepository(ConnectionString);
            Data.DropAndCreateDatabase();
        }

        [TestMethod]
        public void TestGetAll()
        {
            var bin = _repository.Get(1);

            Assert.IsTrue(bin.STATION_ID.HasValue && bin.STATION_ID.Value == 1);

            bin.STATION_ID = null;
            _repository.Update(bin);

            var newBin = _repository.Get(1);

            Assert.IsTrue(!newBin.STATION_ID.HasValue);

        }

        [TestMethod]
        public void l()
        {

        }
    }
}
