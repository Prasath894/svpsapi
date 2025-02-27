using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BatchSubjectFacultyModel
    {
        public long Id { get; set; }
        public long sectionID { get; set; }
        public string SectionName { get; set; }
        public string GradeorClass { get; set; }
        public string FacultyName { get; set; }
        public string Section { get; set; }
        public long SubjectID { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public long FacultyID { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        
    }
}
