
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AlumniResponse
    {
         public string Event { get; set; }
        public string OrganisedBy { get; set; }
        //public string Department { get; set; }
        public string Venue { get; set; }
        public string EventDate { get; set; }
        public string Time { get; set; }
        public string ResourcePersonOrActed { get; set; }
        public string Impact { get; set; }        
       

        public List<Alumni> AlumniID {get;set;}

        public List<string> FileBlob { get; set; }

    }
}
