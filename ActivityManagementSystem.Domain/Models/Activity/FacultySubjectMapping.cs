using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class FacultySubjectMapping
    {
        public long Id { get; set; }
        public string FacultyId { get; set; }
        public long SubjectId { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string facultyName { get; set; } 
        public string Sem { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string SubjectShortForm { get; set; }
        public string Year { get; set; }
        public string Section { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }


       
    }
}
