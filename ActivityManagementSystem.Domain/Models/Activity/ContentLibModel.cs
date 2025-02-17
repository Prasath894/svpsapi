using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ContentLibModel
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
     
        public int Year { get; set; }
        public string Sem { get; set; }
        public string Section { get; set; }
        public string Title { get; set; }
        public string FacultyRollNo { get; set; }

        public string FacultyName { get; set; }
        public string DepartmentName { get; set; }
        public int FacultyId { get; set; }
        public string? Description { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public List<string>? FileList { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
