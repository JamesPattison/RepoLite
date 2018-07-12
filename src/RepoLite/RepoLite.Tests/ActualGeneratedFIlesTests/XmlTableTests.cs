using Microsoft.VisualStudio.TestTools.UnitTesting;
using NS;
using NS.Base;
using NS.Models;
using RepoLite.Tests.ActualGeneratedFIlesTests.Base;
using System.Linq;
using System.Xml;

namespace RepoLite.Tests.ActualGeneratedFIlesTests
{
    [TestClass]
    public class XmlTableTests : BaseTests
    {
        private IXmltableRepository _repository;

        [TestInitialize]
        public void TestInitialize()
        {
            _repository = new XmltableRepository(ConnectionString);
            Data.DropAndCreateDatabase();
        }

        [TestMethod]
        public void TestGetAll()
        {
            var expected = 5;
            var actual = _repository.GetAll().Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml1_Data_Count()
        {
            var expected = 1;
            var actual = _repository.FindByData("<xml>Value</xml>").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml1_Data_Name()
        {
            var expected = "XML1";
            var actual = _repository.FindByData("<xml>Value</xml>").FirstOrDefault()?.Name;

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml1_Name_Count()
        {
            var expected = 1;
            var actual = _repository.FindByName("XML1").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml1_Name_Data()
        {
            var expected = "<xml>Value</xml>";
            var actual = _repository.FindByName("XML1").FirstOrDefault()?.Data;

            Assert.IsTrue(actual.InnerXml == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml2_Data_Count()
        {
            var expected = 1;
            var actual = _repository.FindByData("<xml>Another Value</xml>").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml2_Data_Name()
        {
            var expected = "XML2";
            var actual = _repository.FindByData("<xml>Another Value</xml>").FirstOrDefault()?.Name;

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml2_Name_Count()
        {
            var expected = 1;
            var actual = _repository.FindByName("XML2").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml2_Name_Data()
        {
            var expected = "<xml>Another Value</xml>";
            var actual = _repository.FindByName("XML2").FirstOrDefault()?.Data;

            Assert.IsTrue(actual.InnerXml == expected, $"expected: {expected}, but received: {actual}");
        }


        [TestMethod]
        public void TestFindByData_Xml3_Data_Count()
        {
            var expected = 1;
            var actual = _repository.FindByData("<xml>Yet Another Value</xml>").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml3_Data_Name()
        {
            var expected = "XML3";
            var actual = _repository.FindByData("<xml>Yet Another Value</xml>").FirstOrDefault()?.Name;

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml3_Name_Count()
        {
            var expected = 1;
            var actual = _repository.FindByName("XML3").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml3_Name_Data()
        {
            var expected = "<xml>Yet Another Value</xml>";
            var actual = _repository.FindByName("XML3").FirstOrDefault()?.Data;

            Assert.IsTrue(actual.InnerXml == expected, $"expected: {expected}, but received: {actual}");
        }


        [TestMethod]
        public void TestFindByData_Xml4_Data_Count()
        {
            var expected = 1;
            var actual = _repository.FindByData("<xml><nest>Nested!</nest></xml>").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml4_Data_Name()
        {
            var expected = "XML4";
            var actual = _repository.FindByData("<xml><nest>Nested!</nest></xml>").FirstOrDefault()?.Name;

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml4_Name_Count()
        {
            var expected = 1;
            var actual = _repository.FindByName("XML4").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml4_Name_Data()
        {
            var expected = "<xml><nest>Nested!</nest></xml>";
            var actual = _repository.FindByName("XML4").FirstOrDefault()?.Data;

            Assert.IsTrue(actual.InnerXml == expected, $"expected: {expected}, but received: {actual}");
        }


        [TestMethod]
        public void TestFindByData_Xml5_Data_Count()
        {
            var expected = 1;
            var actual = _repository.FindByData("<xml><nest><nest>Nested Further!</nest></nest></xml>").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml5_Data_Name()
        {
            var expected = "XML5";
            var actual = _repository.FindByData("<xml><nest><nest>Nested Further!</nest></nest></xml>").FirstOrDefault()?.Name;

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml5_Name_Count()
        {
            var expected = 1;
            var actual = _repository.FindByName("XML5").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Xml5_Name_Data()
        {
            var expected = "<xml><nest><nest>Nested Further!</nest></nest></xml>";
            var actual = _repository.FindByName("XML5").FirstOrDefault()?.Data;

            Assert.IsTrue(actual.InnerXml == expected, $"expected: {expected}, but received: {actual}");
        }


        [TestMethod]
        public void TestFindByData_Data_Contains_Value()
        {
            var expected = 3;
            var actual = _repository.FindByData(FindComparison.Like, "Value").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Data_Contains_xml()
        {
            var expected = 5;
            var actual = _repository.FindByData(FindComparison.Like, "xml").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestFindByData_Data_Contains_nest()
        {
            var expected = 2;
            var actual = _repository.FindByData(FindComparison.Like, "nest").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestSearch_1()
        {
            var expected = 1;
            var actual = _repository.Search("XML1").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestSearch_2()
        {
            var expected = 1;
            var actual = _repository.Search(data: "<xml>Value</xml>").Count();

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");
        }

        [TestMethod]
        public void TestCreate()
        {
            var data = new Xmltable
            {
                Name = "Inserted",
                Data = new XmlDocument { InnerXml = "<xml>Xml</xml>" }
            };

            var expected = true;
            var actual = _repository.Create(data);

            Assert.IsTrue(actual == expected, $"expected: {expected}, but received: {actual}");

            Assert.IsTrue(_repository.GetAll().Count() == 6);
        }

        [TestMethod]
        public void TestWhere_1()
        {
            var expected = 3;
            var actual = _repository.Where("data", Comparison.Like, "Value", typeof(XmlDocument)).Results().ToArray();

            Assert.IsTrue(actual.Length == expected, $"expected: {expected}, but received: {actual.Length}");
        }
    }
}
/*
 *
 *             RunSql("INSERT [dbo].[xmltable] VALUES ('XML1','<xml>Value</xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML2', '<xml>Another Value</xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML3', '<xml>Yet Another Value</xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML4', '<xml><nest>Nested!</nest></xml>')");
            RunSql("INSERT [dbo].[xmltable] VALUES ('XML5', '<xml><nest><nest>Nested Further!</nest></nest></xml>')");

    
        bool Create(Xmltable item);
        bool BulkCreate(List<Xmltable> items);

        Where<Xmltable> Where(string col, Comparison comparison, object val);
        IEnumerable<Xmltable> Where(string query);
 */
