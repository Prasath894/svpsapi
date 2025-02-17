using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class EventsResponse
    {
        public string EventName { get; set; }
        public string Topic { get; set; }
        public string NameOfTheChiefGuest { get; set; }
        public string nParticipants { get; set; }
        public string EventDate { get; set; }
        public string TypeOfEvent { get; set; }
       
        public string Impact { get; set; }
      
        public List<string> FileBlob { get; set; }
    }
}
