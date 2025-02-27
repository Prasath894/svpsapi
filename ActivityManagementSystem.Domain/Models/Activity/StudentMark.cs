using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StudentMark
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string SectionName { get; set; }
        public string GradeOrClass { get; set; }
        public int Section { get; set; }
        public string Data { get; set; }
        public string PreviousMonthAttendance { get; set; }
        public bool IsattendanceRequired { get; set; }
        public bool ReadytosendEmail { get; set; }
        public bool IsParentIntemated { get; set; }
        public DateTime CreatedDate { get; set; }
        public string createdby { get; set; }
        public string TestType { get; set; }
        //public List<MarkDetails> items { get; set; }
    }
        public class MarkDetails
        {
            public string SubjectCode { get; set; }
            public string SubjectName { get; set; }
            public string Marks { get; set; }
        }
    
}
