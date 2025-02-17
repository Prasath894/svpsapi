using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BatchSubjectModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int Year { get; set; }
        public string Sem { get; set; }
    }
}
