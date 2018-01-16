using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace Focus3.CsvTransformer.Tests.Helpers
{
    [TestFixture]
    public class ScratchPad
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly CsTransformTestable _transform;
        private readonly string _inputXmlFilePath;

        public ScratchPad()
        {
            var logRepository = LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.Configure(logRepository, new FileInfo("log.config"));

            _inputXmlFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "CS_20180111.xml");
            _transform = new CsTransformTestable(_inputXmlFilePath);
        }

        [Test, Explicit("Test Data Manipulation")]
        public void SrubData()
        {
            var xDocument = _transform.LoadXDocumentTestable(_inputXmlFilePath);

            // scrub SSN's...
            var ssnElements = xDocument.Root?.Descendants("SSN");

            if (ssnElements == null) throw new InvalidDataException("Cannot find any SSN elements in XML");

            foreach (var ssnElement in ssnElements)
            {
                if (!string.IsNullOrWhiteSpace(ssnElement.Value))
                {
                    ssnElement.Value = GenerateRandomSocial(String.Empty);
                }
            }

            // scrub phone 
            var phoneElements = xDocument.Root?.Descendants("Phone");

            if (phoneElements == null) throw new InvalidDataException("Cannot find any Phone elements in XML");

            foreach (var phoneElement in phoneElements)
            {
                if (!string.IsNullOrWhiteSpace(phoneElement.Value))
                {
                    phoneElement.Value = GenerateRandomPhone();
                }
            }

            // scrub email
            var emailElements = xDocument.Root?.Descendants("Email");

            if (emailElements == null) throw new InvalidDataException("Cannot find any Email elements in XML");

            foreach (var emailElement in emailElements)
            {
                if (!string.IsNullOrWhiteSpace(emailElement.Value))
                {
                    emailElement.Value = GenerateRandomEmail();
                }
            }

            // scrub FirstName
            var fNameElements = xDocument.Root?.Descendants("FirstName");

            if (fNameElements == null) throw new InvalidDataException("Cannot find any FirstName elements in XML");

            foreach (var fNameElement in fNameElements)
            {
                if (!string.IsNullOrWhiteSpace(fNameElement.Value))
                {
                    fNameElement.Value = GenerateRandomName();
                }    
            }

            // scrub LastName
            var lNameElements = xDocument.Root?.Descendants("LastName");

            if (lNameElements == null) throw new InvalidDataException("Cannot find any LastName elements in XML");

            foreach (var lNameElement in lNameElements)
            {
                if (!string.IsNullOrWhiteSpace(lNameElement.Value))
                {
                    lNameElement.Value = GenerateRandomName();
                }
            }

            // scrub ExternalEmployeeId
            var idElements = xDocument.Root?.Descendants("ExternalEmployeeId");

            if (idElements == null) throw new InvalidDataException("Cannot find any ExternalEmployeeId elements in XML");

            foreach (var idElement in idElements)
            {
                if (!string.IsNullOrWhiteSpace(idElement.Value))
                {
                    idElement.Value = GenerateRandomId();
                }
            }

            File.WriteAllText(_inputXmlFilePath, xDocument.ToString());
        }

        private static string GenerateRandomSocial(string delimiter)
        {
            int iThree = GetRandomNumber(132, 921);
            int iTwo = GetRandomNumber(12, 83);
            int iFour = GetRandomNumber(1423, 9211);
            return iThree + delimiter + iTwo + delimiter + iFour;
        }

        private static string GenerateRandomPhone()
        {
            int iThree = GetRandomNumber(100, 999);
            int iThree2 = GetRandomNumber(100, 999);
            int iFour = GetRandomNumber(1000, 9999);
            return $"({iThree}) {iThree2}-{iFour}";
        }

        private static string GenerateRandomEmail()
        {
            var prefix = GetRandomNumber(100000, 999999);
            return $"{prefix}@email.com";
        }

        private static string GenerateRandomName()
        {
            return Names[GetRandomNumber(0, 9)];
        }

        private static string GenerateRandomId()
        {
            return GetRandomNumber(100000, 999999).ToString();
        }

        private static readonly Random getrandom = new Random();
        public static int GetRandomNumber(int min, int max)
        {
            return getrandom.Next(min, max);
        }

        private static readonly string[] Names =
            {"Suzy", "Haley", "Jim", "John", "Jack", "Bruce", "Paul", "Richard", "Sally", "Linda"};
    }
}