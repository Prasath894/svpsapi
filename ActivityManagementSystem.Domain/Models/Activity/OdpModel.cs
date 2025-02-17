using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class OdpModel
    {
        public long RefNo { get; set; }
        public DateTime FormDate { get; set; }
        public string CollegeType { get; set; }
        public long ProgrammeName { get; set; }
        public string DepartmentName { get; set; }
        public string DoteType { get; set; }
        public string ExamSession { get; set; }
        public string DoteReference { get; set; }
        public string ExaminersInfo { get; set; }
        public string Note { get; set; }
        public string NameOfExam { get; set; }
        public string DiscriptionOfOthers { get; set; }

        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public long NoOfDays { get; set; }
        public string Maker { get; set; }
        public string Checker1 { get; set; }
        public string Checker2 { get; set; }
        public bool IsMakerCompleted { get; set; }
        public string MakerStatus { get; set; }
        public bool IsChecker1Completed { get; set; }
        public bool IsChecker2Completed { get; set; }
        public string Checker1Status { get; set; }
        public string Checker2Status { get; set; }
        public DateTime MakerDate { get; set; }
        public DateTime Checker1Date { get; set; }
        public DateTime Checker2Date { get; set; }
        public List<string> Files { get; set; }
        public string CurrentFileQueueStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Photo { get; set; }
        public string FileNames { get; set; }
        public DateTime ReleivingOrderIssuedOn { get; set; }
        public string ReleivingOrderNo { get; set; }


    }
}
