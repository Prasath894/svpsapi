using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class GrantsResponse
    {
        public string ProjectName { get; set; }
        public string GrantGivenBy { get; set; }
        public string Department { get; set; }
        public string SanctionedYear { get; set; }
        public string SanctionedBy { get; set; }
       
        public string RecurringAmount { get; set; }
        public string SanctionedOrderNo { get; set; }
    
        public string SanctionedGrantAmount { get; set; }
        public string NonRecurringAmount { get; set; }
       
        public List<Faculty> FacultyID { get; set; }
        public List<string> FileBlob { get; set; }

    }
}
