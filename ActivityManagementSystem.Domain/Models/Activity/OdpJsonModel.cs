using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class OdpJsonModel
    {
        public long sNo { get; set; }
        public string nameofFaculty { get; set; }
        public string doteStaffId { get; set; }
        public string dutyAs { get; set; }
        public string duty { get; set; }

        public string valuationCentre { get; set; }
        public string designation { get; set; }
        public string departmentName { get; set; }
        public string collegeCodeandName { get; set; }
        public string collegeAddress { get; set; }
        public string subject { get; set; }

    }
}
