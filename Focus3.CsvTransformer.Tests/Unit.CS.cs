using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Focus3.CsvTransformer.Tests.Helpers;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace Focus3.CsvTransformer.Tests
{
    [TestFixture]
    public class CsUnit
    {
        private const string Focus3CompanyName = "Focus 3 Benefits";
        private const string CsCompanyName = "CxHQx";
        private const string CsCompanyId = "1xx4";

        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CsTransform _transform;
        private readonly XDocument _xDocument;
        private readonly XElement _companyElement;

        public CsUnit()
        {
            var logRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(logRepository, new FileInfo("log.config"));

            var inputXmlFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "CS_20180111.xml");
            _transform = new CsTransform(inputXmlFile);

            _xDocument = _transform.LoadXDocumentTestable(inputXmlFile);

            _companyElement = _transform.LoadXDocumentTestable(inputXmlFile).Root?.Descendants("Company")
                .First(c => c.Element("Name")?.Value == Focus3CompanyName);

            Assert.IsNotNull(_companyElement, $"Unable to load Focus3 company element from test file [{inputXmlFile}].");
        }

        [Test]
        public void LoadMappingFileTest_Success()
        {
            var mappingDictionary = _transform.LoadMappingFileTestable("mapping.json");
            Assert.IsNotNull(mappingDictionary);
            Assert.That(mappingDictionary.Count > 0);
            Log.Debug($"There were {mappingDictionary.Count} items in the mapping dictionary.");
        }

        [Test]
        public void LoadHeaderColumnsTest_Success()
        {
            var columns = _transform.LoadHeaderColumns();
            Assert.IsNotNull(columns);
            var columnList = columns.ToList();
            Assert.IsNotEmpty(columnList);
            Assert.That(columnList.Any());
            Log.Debug($"There were {columnList.Count} columns in the header list.");
        }

        [Test]
        public void LoadModelsTest_Success()
        {
            var modelList = _transform.LoadModels().ToList();
            Assert.IsNotEmpty(modelList);
            Assert.That(modelList.Count == 3);
            Assert.That(modelList.First()["Company Name"] == CsCompanyName);
            Assert.That(modelList.First()["Company ID"] == CsCompanyId);
        }

        [Test]
        public void GetElementValue_Success()
        {
            var companyName = _transform.GetElementValueTestable(_companyElement, "Name", "Company Name");

            Assert.That(companyName == Focus3CompanyName);
        }

        [Test]
        public void GetElementValue_Fail()
        {
            var companyName = _transform.GetElementValueTestable(_companyElement, "NameO", "Company Name");

            Assert.IsEmpty(companyName);
        }

        [Test]
        public void BuildModelFromXmlTest_Success()
        {
            var modelList = _transform.BuildModelsFromXmlTestable(_xDocument).ToList();

            Assert.IsNotEmpty(modelList);
            Assert.That(modelList.Count() == 3);
            Assert.That(modelList.First()["Company Name"] == CsCompanyName);
            Assert.That(modelList.First()["Company ID"] == CsCompanyId);
        }
    }
}
