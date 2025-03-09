using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class TimetableModel
    {
        public int Id { get; set; }
        public string Day { get; set; } = string.Empty;
        public string SectionId { get; set; } = string.Empty;
        public string HallNo { get; set; } = string.Empty;



        public string GradeOrClass { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public DateTime WithEffectFrom { get; set; }
        public string Hour1 { get; set; } = string.Empty;
        public string Hour2 { get; set; } = string.Empty;
        public string Hour3 { get; set; } = string.Empty;
        public string Hour4 { get; set; } = string.Empty;
        public string Hour5 { get; set; } = string.Empty;
        public string Hour6 { get; set; } = string.Empty;
        public string Hour7 { get; set; } = string.Empty;
        public string Hour8 { get; set; } = string.Empty;
       
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; } // Nullable
        public DateTime? ModifiedDate { get; set; } // Nullable
    }
}
