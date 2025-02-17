using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BpeModel
    {
        public long RefNo { get; set; }
        public string CollegeType { get; set; }
        public string ReleivingOrderNo { get; set; }
        public string DOTEReference { get; set; }
        public DateTime FormDate { get; set; }
        public string LrNo { get; set; }
        public DateTime ExamStartDate { get; set; }
        public DateTime ExamEndDate { get; set; }
        public long NoOfDays { get; set; }
        public string AppointedAs { get; set; }
        public string AppointedOthers { get; set; }
        public string NameOfThePractical { get; set; }
        public string ExamSession { get; set; }
        public string ExaminersInfo { get; set; }
        public string Maker { get; set; }
        public string Checker1 { get; set; }
        public string Checker2 { get; set; }
        public string Checker3 { get; set; }
        public string Checker4 { get; set; }
        public bool IsMakerCompleted { get; set; }
        public string MakerStatus { get; set; }
        public DateTime MakerDate { get; set; }
        public bool IsChecker1Completed { get; set; }
        public bool IsChecker2Completed { get; set; }
        public string Checker1Status { get; set; }
        public string Checker2Status { get; set; }
        public DateTime Checker1Date { get; set; }
        public DateTime Checker2Date { get; set; }
        public bool IsChecker3Completed { get; set; }
        public string Checker3Status { get; set; }
        public DateTime Checker3Date { get; set; }
        public bool IsChecker4Completed { get; set; }
        public string Checker4Status { get; set; }
        public DateTime Checker4Date { get; set; }
        public string CurrentFileQueueStatus { get; set; }
        public string Note { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<string> files { get; set; }
        public string FileNames { get; set; }
        public string Photo { get; set; }
        public DateTime RelievingOrderIssuedon { get; set; }
        public int MakerOrHodFlag { get; set; }
        public int ProgrammeName { get; set; }
    }
}
