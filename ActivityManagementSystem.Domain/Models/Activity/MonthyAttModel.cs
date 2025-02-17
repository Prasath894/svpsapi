using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class MonthyAttModel
    {
        public int BatchId { get; set; }        
        public long SubjectId { get; set; }
        public int SemTotalHours { get; set; }
        public string BatchName { get; set; }

    }
}
