using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BudgetReallocateModel
    {
        public string BudgetAmtTo { get; set; }
        public string BudgetAmtFrom { get; set; }
        public DateTime budgetDate { get; set; }

        public long SNoFrom { get; set; }

        public long SNoTo { get; set; }
        public string collegeTypeFrom { get; set; }
        //public long HeadOfAccountFrom { get; set; }
        public long HeadOfAccountTo { get; set; }
        //public string collegeTypeTo { get; set; }
        //public long departmentIdFrom { get; set; }
        public long departmentIdTo { get; set; }
        public DateTime ModifiedDate { get; set; }       
        public string ModifiedBy { get; set; }

    }
}
