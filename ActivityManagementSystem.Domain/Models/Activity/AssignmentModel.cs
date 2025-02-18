using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AssignmentModel
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public int SubjectId { get; set; }
        public string Title { get; set; }  // varchar(5)
        public long FacultyId { get; set; }  // varchar(4)
        public string Description { get; set; }  // varchar(50)
        public DateOnly DueDate { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }  // varchar(500)

        public List<string> FileList { get; set; }
        public string GradeOrClass { get; set; }  // varchar(500)
        public string Section { get; set; }  // varchar(500)
        public int CoordinatorId { get; set; }  // varchar(500)
        public string FacultyRegNo { get; set; }  // varchar(500)
        public string FacultyName { get; set; }  // varchar(500)
        public string SubjectCode { get; set; }  // varchar(500)
        public string SubjectName { get; set; }  // varchar(500)

        public string CreatedBy { get; set; }  // varchar(500)

        public string ModifiedBy { get; set; }  // varchar(500)

    }
}
