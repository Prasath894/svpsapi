using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ConsultantResponse
    {

        public string ProjectName { get; set; }
        public string MethodologyOpt { get; set; }
        public string ConsultancyFees { get; set; }      
        public string Organisation { get; set; }       
        public string ProjectGivenDate { get; set; }
        public string ProjectCompletionDate { get; set; }
        public string SupportingStaff { get; set; }
        public string StandardsIfAny { get; set; }
        public string ProblemStatement { get; set; }
    

        public string InternalOrExternal { get; set; }
        public string OnlineOrOffline { get; set; }



        public List<string> FileBlob { get; set; }

        public List<Faculty> FacultyID { get; set; }
    }
}
