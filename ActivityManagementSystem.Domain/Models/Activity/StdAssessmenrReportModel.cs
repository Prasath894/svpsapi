using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StdAssessmenrReportModel
    {
        public int SNo { get; set; }
        public string TimeStamp { get; set; }
        public string RegisterNo { get; set; }
        public string StudentName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Year { get; set; }
        public string Sem { get; set; }
        public string Section { get; set; }
        public string Score { get; set; }

    }
}
