using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StudentAttendanceModel
    {
            public Int64 StudentId { get; set; }
            public string StudentName { get; set; }
          
            public Dictionary<string, string> AttendanceRecords { get; set; } = new Dictionary<string, string>();
        
    }
}
