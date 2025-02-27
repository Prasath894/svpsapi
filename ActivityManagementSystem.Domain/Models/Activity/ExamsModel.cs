using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ExamsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }

    }
}