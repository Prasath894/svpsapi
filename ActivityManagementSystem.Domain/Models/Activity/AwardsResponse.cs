using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AwardsResponse
    {
        public string NameOfTheAwards { get; set; }
       
        public string AwardBestowedTo { get; set; }
        public string NameOfOrganisers { get; set; }
        public string Venue { get; set; }
        public string Event { get; set; }
        
        public string DateOfAnnouncement { get; set; }

        //public string Department { get; set; }
        public string DateOfAwardCeremony { get; set; }
        public string Year { get; set; }
        public string NameOfThePrincipal { get; set; }
        public string NameOfTheInstitution { get; set; }

        public string DescriptionOfAwards { get; set; }
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }
        public List<string> FileBlob { get; set; }
    }
}
