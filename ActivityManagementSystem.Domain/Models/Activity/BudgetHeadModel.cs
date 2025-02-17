using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BudgetHeadModel
    {
        public long SNo { get; set; }
        public DateTime HeadDate { get; set; } 
        public long DepartmentId { get; set; }
        public string HeadAmt { get; set; }
        public string CollegeType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DepartmentName { get; set; }
        public long HeadOfAccount { get; set; }

        
    }
}
