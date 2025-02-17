using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class DepartmentDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsActive { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
