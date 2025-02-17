using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ActivityResponse
    {
        public string Topic { get; set; }       
        public string Event { get; set; }
        public string OrganisedBy { get; set; }
        public string Department { get; set; }
        public string Venue { get; set; }
        public string Prize { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Abstract { get; set; }
        public string Impact { get; set; }
        
        public string StudentOrFaculty { get; set; }
        public string InternalOrExternal { get; set; }
        public string OffLineOnLineOrBlended { get; set; }
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }
       
        public List<string> FileBlob { get; set; }
    }

    public class Data
    {

    }

    public class Student
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Faculty
    {
        public string FacultyID { get; set; }
        public string FacultyName { get; set; }
        public string DepartmentName { get; set; }
    }

    public class Alumni
    {
        public string AlumniID { get; set; }
        public string AlumniName { get; set; }
       
    }

}
