using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class InventoryIssuedMappingModel
    {
        public long Id { get; set; }
        public DateTime IssueDate { get; set; }
        public string CollegeType { get; set; }
        public string Others { get; set; }

        public long InventoryId { get; set; }
        public long DepartmentId { get; set; }
        public string Store { get; set; }
        public string StudentId { get; set; }
        public string FacultyId { get; set; }
        public int Year { get; set; }
        public string AvailableQuantity { get; set; }
        public string IssuedQuantity { get; set; }
        //public string CloseStock { get; set; }
        public long AvailableStock { get; set; }
        public string InventoryName { get; set; }
        public string Specification { get; set; }
        public long NoOfItems { get; set; }
        public string StudentOrFaculty { get; set; }

        public string IssuedBy { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
