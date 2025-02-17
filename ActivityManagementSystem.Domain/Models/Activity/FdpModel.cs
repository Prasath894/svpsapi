using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class FdpModel
    {
        public long RefNo { get; set; }
        public DateTime RelievingOrderIssuedon { get; set; }
        public DateTime FormDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string TitleOfTheProgramme { get; set; }
        public string DescriptionOfOthers { get; set; }
        public string venue { get; set; }
        public string Duration { get; set; }
        public string Note { get; set; }
        public string collegeType { get; set; }

        public string FacultyName { get; set; }
        public long NoOfDaysAttended { get; set; }
        public string ConductedBy { get; set; }
        public string TypeOfProgramme { get; set; }
        public string level { get; set; }
        public long HeadOfAccount { get; set; }

        public string ProgrammeDetails { get; set; }
        public string SubmittedReleivingOrder { get; set; }
        public string SubmittedCopyOfCertificate { get; set; }
        public string TaDaBroneBy{ get; set; }
        public string Maker { get; set; }
        public string Checker1 { get; set; }
        public string Checker2 { get; set; }
        public string Checker3 { get; set; }
        public string Checker4 { get; set; }
        public bool IsMakerCompleted { get; set; }
        public string MakerStatus { get; set; }
        public bool IsChecker1Completed { get; set; }
        public bool IsChecker2Completed { get; set; }
        public bool IsChecker3Completed { get; set; }
        public bool IsChecker4Completed { get; set; }
        public string Checker1Status { get; set; }
        public string Checker2Status { get; set; }
        public string Checker3Status { get; set; }
        public string Checker4Status { get; set; }
        public string Photo { get; set; }
        public int MakerOrHodFlag { get; set; }
        public string FileNames { get; set; }
        public string LevelOthers { get; set; }
        public List<string> Files { get; set; }
        public string CurrentFileQueueStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public  string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public DateTime MakerDate { get; set; }
        public DateTime Checker1Date { get; set; }
        public DateTime Checker2Date { get; set; }
        public DateTime Checker3Date { get; set; }
        public DateTime Checker4Date { get; set; }

    }
}
