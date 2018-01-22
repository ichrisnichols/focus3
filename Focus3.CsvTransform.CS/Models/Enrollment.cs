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
        public DateTime? StartDate { get; set; }
        public DateTime? EnrolledOn { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? EmployeeCost { get; set; }
        public decimal? EmployerCost { get; set; }
    }
}
