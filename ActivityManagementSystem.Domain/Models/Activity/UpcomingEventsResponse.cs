using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
   public class UpcomingEventsResponse
    {
        public string EventName { get; set; }
        public string Topic { get; set; }
        public string NameOfTheChiefGuest { get; set; }
        public string NameOfTheChiefGuest2 { get; set; }
        public string NameOfTheChiefGuest3 { get; set; }
        public string NameOfTheChiefGuest4 { get; set; }
        public string NameOfTheChiefGuest5 { get; set; }

        public string nParticipants { get; set; }
        public string EventDate { get; set; }
        public string TypeOfEvent { get; set; }


        public List<string> FileBlob { get; set; }
    }
    
}
