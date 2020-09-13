using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using NS.Models;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class BinManTests : BaseTests
    {
        private IBinManRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            Data.DropAndCreateDatabase();
            _repository = new BinManRepository(ConnectionString);
            
            var binMan = new BinMan
            {
                Id = 1,
                Data = new byte[]
                {
                    1, 2, 3
                }
            };
            _repository.Create(binMan);
        }
        
        [TestMethod]
        public void TestCreateBinMan()
        {
            var binMan = new BinMan
            {
                Id = 123,
                Data = new byte[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8
                }
            };
            var created = _repository.Create(binMan);
            
            Assert.IsTrue(created);

            var all = _repository.GetAll();
            var retrieved = all.First(x => x.Id != 1);
            Assert.IsTrue(retrieved.Id == 123);
            Assert.IsTrue(retrieved.Data.Length == 8);
        }
        
        [TestMethod]
        public void TestDeleteBinMan()
        {
            Assert.IsTrue(_repository.GetAll().Count() == 1);
            
            var deleted = _repository.DeleteById(1);
            Assert.IsTrue(deleted);
            Assert.IsTrue(!_repository.GetAll().Any());
        }
    }
}