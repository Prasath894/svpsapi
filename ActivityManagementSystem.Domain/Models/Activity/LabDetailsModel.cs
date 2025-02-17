using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class LabDetailsModel
    {
        public long SINo { get; set; }
        public string CollegeType{get; set;}
        public long DepartmentId { get; set; }

        
        public string DepartmentName { get; set; }
        public string LabName { get; set; }
        public string CreatedBy{get; set;}
        public DateTime CreatedDate { get; set;}
        public string ModifiedBy{get; set;}
        public DateTime ModifiedDate { get; set; }
    }
}
