using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class PatentResponse
    {

        public string PatentTitle { get; set; }
        public string PatentType { get; set; }
        public string PatentRegistrationDetails { get; set; }
        public string Mentor { get; set; }

        public string Abstract { get; set; }
        public string Status { get; set; }
        public string NationalOrInternational { get; set; }
        public string StudentOrFaculty { get; set; }
        //public string Department { get; set; }
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }
        public List<string> FileBlob { get; set; }

    }
}
