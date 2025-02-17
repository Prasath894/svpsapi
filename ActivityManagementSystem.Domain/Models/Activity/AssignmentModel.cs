using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AssignmentModel
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public int Year { get; set; }
        public string Sem { get; set; }  // varchar(5)
        public string Section { get; set; }  // varchar(4)
        public string Title { get; set; }  // varchar(50)
        public int FacultyId { get; set; }
        public int SubjectId { get; set; }
        public string DepartmentName { get; set; }  // varchar(500)
        public string DepartmentCode { get; set; }  // varchar(500)
        public string FacultyRegNo { get; set; }  // varchar(500)
        public string FacultyName { get; set; }  // varchar(500)
        public string SubjectCode { get; set; }  // varchar(500)
        public string SubjectShortForm { get; set; }  // varchar(500)
        public string SubjectName { get; set; }  // varchar(500)

        public string Description { get; set; }  // varchar(500)
        public DateTime DueDate { get; set; }

        public string FileName { get; set; }  // varchar(100)
        public string FilePath { get; set; }  // varchar(100)
       
        public List<string>? FileList { get; set; }

        public string CreatedBy { get; set; }  // varchar(150)
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }  // varchar(150)
        public DateTime? ModifiedDate { get; set; }  // Nullable for datetime
    }
}
