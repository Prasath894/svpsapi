using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AttendanceModel
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string StudentName { get; set; }
        public string SubjectName { get; set; }
        public string SubjectCode { get; set; }
        public string RollNo { get; set; }
        public int SubjectId { get; set; }
        public long StudentId { get; set; }
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }
        public int Hoursday { get; set; }
        public string Hoursdays { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
