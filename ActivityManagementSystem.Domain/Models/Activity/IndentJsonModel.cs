using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class IndentJsonModel
    {
        public long sNo { get; set; }

        public string descriptionofItems { get; set; }
        //public string tentativeCost { get; set; }       
        public string qtyRequired { get; set; }
        public string costPerUnit { get; set; }
        public string totalBudget { get; set; }
        public string budget { get; set; }
        public string units { get; set; }

        
        public string cumulativeExpenses { get; set; }

    }
}
