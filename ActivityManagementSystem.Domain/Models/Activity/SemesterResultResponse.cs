
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class SemesterResultResponse
    {

        public string Sem { get; set; }
        public string Year { get; set; }
        public string Department { get; set; }
        public string SemMonth { get; set; }
        public string PassPercentage { get; set; }
        public string NStudentAppeared { get; set; }
        public string NStudentPassed { get; set; }
        public string NBoysAppeared { get; set; }       
        public string NGirlsAppeared { get; set; }
        public string NBoysPassed { get; set; }
        public string NGirlsPassed { get; set; }       
        public string Remarks { get; set; }
        
        public List<string> FileBlob { get; set; }

    }
}
