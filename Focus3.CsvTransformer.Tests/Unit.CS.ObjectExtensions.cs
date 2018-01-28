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
            const string address1 = "123 Main St.";
            const string city = "Los Angeles";
            const string state = "CA";
            const string companyName = "XXX";
            const string companyId = "Shebang";
            const string employeeId = "123";
            const string employeeSsn = "111-22-3333";
            const string dependentSsn = "444-55-6666";
            const string status = "Active";
            const string fName = "Bob";
            const string lName = "Marley";
            const string email = "bob@marley.com";
            const string carrier = "Cyberius";
            const string planName = "Premium Plan";
            const string relationship = "child";
            const string seqNum = "1";
            const string enrollType = "Current";

            // employee
            var employee = new Employee
            {
                Address = new Address {Address1 = address1, City = city, State = state},
                Company = new Company {Id = companyId, Name = companyName},
                EmployeeId = employeeId,
                EmployeeSsn = employeeSsn,
                HireDate = DateTime.Today,
                TermDate = DateTime.MaxValue,
                Status = status,
                CommonPersonalData = new CommonPersonalData {FirstName = fName, LastName = lName, Email = email},
                Enrollment = new Enrollment { Carrier = carrier, PlanName = planName, EnrollmentType = enrollType}
            };

            var dictionary = employee.AsDictionary();

            Assert.IsNotNull(dictionary);
            Assert.That(dictionary.Keys.Count > 0);
            Assert.That((DateTime)dictionary["HireDate"] == DateTime.Today);
            Assert.That((DateTime)dictionary["TermDate"] == DateTime.MaxValue);
            Assert.That((string)dictionary["EmployeeId"] == employeeId);
            Assert.That((string)dictionary["EmployeeSsn"] == employeeSsn);
            Assert.That((string)dictionary["Status"] == status);
            // company data
            Assert.That((string)dictionary["Id"] == companyId);
            Assert.That((string)dictionary["Name"] == companyName);
            // common personal data
            Assert.That((string)dictionary["FirstName"] == fName);
            Assert.That((string)dictionary["LastName"] == lName);
            Assert.That((string)dictionary["Email"] == email);
            // address data
            Assert.That((string)dictionary["Address1"] == address1);
            Assert.That((string)dictionary["City"] == city);
            Assert.That((string)dictionary["State"] == state);
            // enrollment data
            Assert.That((string)dictionary["Carrier"] == carrier);
            Assert.That((string)dictionary["PlanName"] == planName);
            // dependent only fields
            Assert.That(!dictionary.ContainsKey("Relationship"));
            Assert.That(!dictionary.ContainsKey("SequenceNum"));
            Assert.That(!dictionary.ContainsKey("Ssn"));
            // these complex types should not mapped 
            Assert.That(dictionary["Enrollment"] == null);
            Assert.That(dictionary["Company"] == null);
            Assert.That(dictionary["Address"] == null);
            Assert.That(dictionary["CommonPersonalData"] == null);

            // dependent
            var enrollee = new Dependent
            {
                Address = new Address { Address1 = address1, City = city, State = state },
                Company = new Company { Id = companyId, Name = companyName },
                EmployeeId = employeeId,
                EmployeeSsn = employeeSsn,
                Relationship = relationship,
                Ssn = dependentSsn,
                SequenceNum = seqNum,
                CommonPersonalData = new CommonPersonalData { FirstName = fName, LastName = lName, Email = email },
            };

            dictionary = enrollee.AsDictionary();

            Assert.IsNotNull(dictionary);
            Assert.That(dictionary.Keys.Count > 0);
            // employee onll fields
            Assert.That(!dictionary.ContainsKey("Enrollment"));
            Assert.That(!dictionary.ContainsKey("HireDate"));
            Assert.That(!dictionary.ContainsKey("TermDate"));
            Assert.That(!dictionary.ContainsKey("Status"));
            Assert.That(!dictionary.ContainsKey("Carrier"));
            Assert.That(!dictionary.ContainsKey("PlanName"));
            // employee & dependent fields
            Assert.That((string)dictionary["EmployeeId"] == employeeId);
            Assert.That((string)dictionary["EmployeeSsn"] == employeeSsn);
            // company data
            Assert.That((string)dictionary["Id"] == companyId);
            Assert.That((string)dictionary["Name"] == companyName);
            // common personal data
            Assert.That((string)dictionary["FirstName"] == fName);
            Assert.That((string)dictionary["LastName"] == lName);
            Assert.That((string)dictionary["Email"] == email);
            // address data
            Assert.That((string)dictionary["Address1"] == address1);
            Assert.That((string)dictionary["City"] == city);
            Assert.That((string)dictionary["State"] == state);
            // these complex types should not mapped 
            Assert.That(dictionary["Company"] == null);
            Assert.That(dictionary["Address"] == null);
            Assert.That(dictionary["CommonPersonalData"] == null);
        }
    }
}
