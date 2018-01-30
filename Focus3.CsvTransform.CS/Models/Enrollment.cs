using System;
using System.Collections.Generic;
using System.Text;

namespace Focus3.CsvTransform.CS.Models
{
    public class Enrollment
    {
        public string EnrollmentType { get; set; }
        public string Carrier { get; set; }
        public string CarrierPlanCode { get; set; }
        public string PlanName { get; set; }
        public string CoverageLevel { get; set; }
        public string StartDate { get; set; }
        public string EnrolledOn { get; set; }
        public string EndDate { get; set; }
        public string EmployeeCost { get; set; }
        public string EmployerCost { get; set; }
    }
}
