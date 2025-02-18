using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class SubjectModel
    {
        public int Id { get; set; }
        public string SubjectShortForm { get; set; }

        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Grade { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
    public class SubjectAttendanceModel
    {
        public long Id { get; set; }
        public string SubjectShortForm { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
    }
    }
