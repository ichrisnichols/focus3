using System;
using System.Collections.Generic;
using System.Text;

namespace Focus3.CsvTransform.CS.Models
{
    public class Enrollee
    {
        public Company Company { get; set; }
        public string EmployeeSsn { get; set; }
        public string EmployeeId { get; set; }
        public CommonPersonalData CommonPersonalData { get; set; }
        public Address Address { get; set; }
    }

    public class Employee : Enrollee
    {
        public DateTime? HireDate { get; set; }
        public string Status { get; set; }
        public DateTime? TermDate { get; set; }
        public Enrollment Enrollment { get; set; }
    }

    public class Dependent : Enrollee
    {
        public string Ssn { get; set; }
        public string Relationship { get; set; }
        public string SequenceNum { get; set; }
    }

    public class CommonPersonalData
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }
}
