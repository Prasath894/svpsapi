using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class IndustrialVisitResponse
    {
        public string OrganisationName { get; set; }
        public string Visit { get; set; }
        public string DateToVisit { get; set; }
        public string OrganisationAddress { get; set; }
        public int NParticipants { get; set; }
        public string Department { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; } 
        public string Remarks { get; set; }
        public string Impact { get; set; }    
        public string StudentOrFaculty { get; set; }
        
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }
        public List<string> FileBlob { get; set; }

    }

}
