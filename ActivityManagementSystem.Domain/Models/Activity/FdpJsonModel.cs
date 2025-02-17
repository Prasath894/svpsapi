using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class FdpJsonModel
    {
        public long sNo { get; set; }
        public string nameofFaculty { get; set; }
        public string designation { get; set; }
        public string departmentName { get; set; }

        public string registrationFees { get; set; }
        public string appexTAandDA { get; set; }
        public string totalBudget { get; set; }

    }
}
