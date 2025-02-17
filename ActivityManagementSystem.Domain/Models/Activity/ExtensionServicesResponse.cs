using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ExtensionServicesResponse
    {
       
        public string Topic { get; set; }
        public string Participants { get; set; }
        public long NParticipants { get; set; }
        public long NParticipantsBoys { get; set; }
        public long NParticipantsGirls { get; set; }
        public string Department { get; set; }
        public string OrganisedBy { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }

        public decimal CourseFees { get; set; }
        public string ResourcePersonDetails { get; set; }

        public string Remarks { get; set; }
        public string Impact { get; set; }
        public string TotalRevenueGenerated { get; set; }
      
        public List<string> FileBlob { get; set; }


    }
}
