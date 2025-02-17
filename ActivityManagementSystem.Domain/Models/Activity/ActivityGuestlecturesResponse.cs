using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ActivityGuestlecturesResponse
    {
        
        public string Topic { get; set; }
        public string ResourcePerson { get; set; }
        public string ResourcePersonDetails { get; set; }
        public string OrganisedBy { get; set; }
        public string Department { get; set; }
        public string ParticipantsFrom { get; set; }        
        public string NParticipantsGirls { get; set; }
        public string NParticipantsBoys { get; set; }
        public string ExternalParticipants { get; set; }
        public string NParticipants { get; set; }
        public string FromDate { get; set; }
        public string Time { get; set; }
        
        
       
        public string OnlineOrOffline { get; set; }
        
        public List<string> FileBlob { get; set; }

    }


}
