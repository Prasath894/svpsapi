using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
     public class InventorySpecModel
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public string InventoryName { get; set; }
        public string Store { get; set; }
        public string Description { get; set; }
        public string Specification { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
