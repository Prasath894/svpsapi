using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BudgetLineModel
    {
        public long SNo { get; set; }
        public DateTime BudgetDate { get; set; }
        public string collegeType { get; set; }
        public long DepartmentId { get; set; }
        public string BudgetAmt { get; set; }
        //public string UpdatedAmt { get; set; }
        public string HeadAmt { get; set; }
        public string OldAmt { get; set; }
        public long HeadOfAccount { get; set; }
        public string HeadOfTheAccount { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DepartmentName { get; set; }

    }
}
