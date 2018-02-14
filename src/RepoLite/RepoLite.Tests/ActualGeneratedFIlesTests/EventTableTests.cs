using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class EventTableTests : BaseTests
    {
        private IEventRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new EventRepository(ConnectionString);
            Data.DropAndCreateDatabase();
        }

    }
}
