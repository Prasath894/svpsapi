using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BatchStudMappingModel
    {
        public int Id { get; set; }
        public string SectionName { get; set; }
        public int SectionId { get; set; }
        public string GradeOrClass { get; set; }        
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public string AdmissionNumber { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
      
    }
}
