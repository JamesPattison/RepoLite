using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class PersonTableTests : BaseTests
    {
        private IPersonRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new PersonRepository(ConnectionString);
            Data.DropAndCreateDatabase();
        }

    }
}
