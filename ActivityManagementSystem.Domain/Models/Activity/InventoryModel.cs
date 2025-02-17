using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public  class InventoryModel
    {
        public long Id { get; set; }
         public string Name { get; set; }
        public string Store { get; set; }
       
        public bool IsActive{ get; set; }
        public string CreatedBy{ get; set; }
        public DateTime CreatedDate{ get; set; }
        public string ModifiedBy{ get; set; }
        public DateTime ModifiedDate{ get; set; }
    }
}
