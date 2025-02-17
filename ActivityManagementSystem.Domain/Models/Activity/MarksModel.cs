using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class MarksModel
    {
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Data { get; set; }
        public string PreviousMonthAttendance { get; set; }
        public bool IsattendanceRequired { get; set; }

        public string Father_MobileNumber { get; set; }
    }
}
