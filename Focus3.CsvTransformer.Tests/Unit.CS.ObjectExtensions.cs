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
            var employee = new Employee
            {
                Address = new Address {Address1 = "123 Main St.", City = "Los Angeles", State = "CA"},
                Company = new Company {Id = "XXX", Name = "Shebang"},
                EmployeeId = "123",
                EmployeeSsn = "111-22-3333",
                Status = "Active",
                CommonPersonalData = new CommonPersonalData {FirstName = "Bob", LastName = "Marley", Email = "bob@marley.com"},
                Enrollment = new Enrollment { Carrier = "Cyberius", PlanName = "Premium Plan"}
            };

            var enrollee = new Enrollee
            {
                Address = new Address { Address1 = "123 Main St.", City = "Los Angeles", State = "CA" },
                Company = new Company { Id = "XXX", Name = "Shebang" },
                EmployeeId = "123",
                EmployeeSsn = "111-22-3333",
                CommonPersonalData = new CommonPersonalData { FirstName = "Bob", LastName = "Marley", Email = "bob@marley.com" }
            };

            var dictionary = employee.AsDictionary();

            Assert.IsNotNull(dictionary);
            Assert.That(dictionary.Keys.Count > 0);

            dictionary = enrollee.AsDictionary();

            Assert.IsNotNull(dictionary);
            Assert.That(dictionary.Keys.Count > 0);
        }
    }
}
