using System;
using System.Collections.Generic;
using System.Text;
using Focus3.CsvTransform.CS.Extensions;
using Focus3.CsvTransform.CS.Models;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Focus3.CsvTransformer.Tests
{
    [TestFixture]
    public class CsObjectExtensionUnit
    {
        [Test]
        public void AsDictionaryTest_Success()
        {
            // employee
            var employee = new Employee
            {
                Address = new Address {Address1 = "123 Main St.", City = "Los Angeles", State = "CA"},
                Company = new Company {Id = "XXX", Name = "Shebang"},
                EmployeeId = "123",
                EmployeeSsn = "111-22-3333",
                HireDate = DateTime.Today,
                Status = "Active",
                CommonPersonalData = new CommonPersonalData {FirstName = "Bob", LastName = "Marley", Email = "bob@marley.com"},
                Enrollment = new Enrollment { Carrier = "Cyberius", PlanName = "Premium Plan"}
            };

            var dictionary = employee.AsDictionary();

            Assert.IsNotNull(dictionary);
            Assert.That(dictionary.Keys.Count > 0);
            Assert.That((DateTime)dictionary["HireDate"] == DateTime.Today);
            Assert.That((string)dictionary["EmployeeId"] == "123");
            Assert.That((string)dictionary["EmployeeSsn"] == "111-22-3333");
            Assert.That((string)dictionary["Status"] == "Active");
            Assert.That(dictionary["Enrollment"] == null);
            Assert.That(dictionary["Company"] == null);
            Assert.That(dictionary["Address"] == null);
            Assert.That(dictionary["CommonPersonalData"] == null);

            // enrollee
            var enrollee = new Enrollee
            {
                Address = new Address { Address1 = "123 Main St.", City = "Los Angeles", State = "CA" },
                Company = new Company { Id = "XXX", Name = "Shebang" },
                EmployeeId = "123",
                EmployeeSsn = "111-22-3333",
                CommonPersonalData = new CommonPersonalData { FirstName = "Bob", LastName = "Marley", Email = "bob@marley.com" }
            };

            dictionary = enrollee.AsDictionary();

            Assert.IsNotNull(dictionary);
            Assert.That(dictionary.Keys.Count > 0);
        }
    }
}
