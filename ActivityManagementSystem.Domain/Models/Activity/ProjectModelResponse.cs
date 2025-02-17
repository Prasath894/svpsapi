using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ProjectModelResponse
    {

       
        public string Topic { get; set; }
        public string Abstract { get; set; }
        public string Event { get; set; }
        public string OrganisedBy { get; set; }
        public string Venue { get; set; }
        public string ProjectTitle { get; set; }
        public string MentorName { get; set; }
        public string ProjectDescription { get; set; }
        public string Mode { get; set; }
        public decimal ProjectCostApprox { get; set; }
        public string FutureScope { get; set; }
        public string Type { get; set; }
        public string DesignType { get; set; }
        public string Prize { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Impact { get; set; }
        //public string Department { get; set; }
        public string StudentOrFaculty { get; set; }
        public string InternalOrExternal { get; set; }
       
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }
        public List<string> FileBlob { get; set; }
    }
}
