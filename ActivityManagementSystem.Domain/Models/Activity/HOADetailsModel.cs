using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
     public class HOADetailsModel
    {
        public long SINo { get; set; }
        public string CollegeType { get; set; }
        public string HeadOfAccount { get; set; }
        public bool IsActiveFdp { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
