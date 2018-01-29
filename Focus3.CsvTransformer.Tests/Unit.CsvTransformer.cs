using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Focus3.CsvTransform.CS;
using Focus3.CsvTransformer.Tests.Helpers;
using log4net;
using log4net.Config;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Focus3.CsvTransformer.Tests
{
    [TestFixture]
    public class CsvTransformerUnit
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private CsvTransformerTestable _csvTransformer;
        private CsTransform _csTransform;

        public CsvTransformerUnit()
        {
            var logRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(logRepository, new FileInfo("log.config"));
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            var inputXmlFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "CS_20180111.xml");

            _csTransform = new CsTransform(inputXmlFilePath);
            _csvTransformer = new CsvTransformerTestable(_csTransform);
        }

        [Test]
        public void GenerateCsvTest_Success()
        {
            const string companyName = "XXX";
            const string companyId = "Shebang";
            const string employeeId = "123";
            const string employeeSsn = "111-22-3333";
            const string dependentSsn = "444-55-6666";
            const string carrier = "Cyberius";

            const string expectedHeaderData = "Company ID,Company Name,Enrollment Type,Employee SSN,Dependent SSN,Relationship,External ID,Sequence Number,Last Name,First Name,Middle Name,Suffix,Date of Birth,Gender,Hire Date,Status,Termination Date,Email,Phone,Address 1,Address 2,City,State,Zip Code,Carrier,Carrier Plan Code,Plan Name,Coverage Level,Start Date,Enrolled On,End Date,Employee Cost,Employer Cost";
            const string expectedLineData = "Shebang,XXX,,111-22-3333,444-55-6666,,123,,,,,,,,,,,,,,,,,,Cyberius,,,,,,,,,";

            const string filename = "test.csv";
            var testOutputFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, filename);

            var models = new List<IDictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    {"Id", companyId},
                    {"Name", companyName},
                    {"EmployeeId", employeeId},
                    {"EmployeeSsn", employeeSsn},
                    {"Ssn", dependentSsn},
                    {"Carrier", carrier}
                },
                new Dictionary<string, object>
                {
                    {"Id", companyId},
                    {"Name", companyName},
                    {"EmployeeId", employeeId},
                    {"EmployeeSsn", employeeSsn},
                    {"Ssn", dependentSsn},
                    {"Carrier", carrier}
                }
            };

            _csvTransformer.GenerateCsvTestable(testOutputFilePath, _csTransform.LoadHeaderColumnMappings(), models);

            var fileLines = File.ReadLines(testOutputFilePath);

            Assert.IsNotNull(fileLines);
            var lineList = fileLines.ToArray();
            Assert.IsNotEmpty(lineList);
            Assert.That(lineList.Count() == 3);
            Assert.That(lineList[0] == expectedHeaderData);
            Assert.That(lineList[1] == expectedLineData);
            Assert.That(lineList[2] == expectedLineData);
        }
    }
}
