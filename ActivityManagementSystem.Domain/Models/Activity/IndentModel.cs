using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class IndentModel
    {
        public long RefNo { get; set; }
        public string ReferenceNo { get; set; }

        public DateTime FormDate { get; set; }
        public long NameOfTheProgramme { get; set; }
        public long NameOfTheLaboratory { get; set; }
        public long HeadOfAccount { get; set; }
        public string purpose { get; set; }
        public string NameOfTheProject { get; set; }
        public string nature { get; set; }
        public string Note { get; set; }
        public string CollegeType { get; set; }
        public string HeadName { get; set; }
        public string LabName { get; set; }
        public string DepartmentName { get; set; }
        public string requirement { get; set; }
        public string productInfo { get; set; }      
        public string Maker { get; set; }
        public string MakerStatus { get; set; }

        public string currentFileQueueStatus { get; set; }

        public string Checker1Status { get; set; }
        public string Checker2Status { get; set; }
        public string Checker3Status { get; set; }
        public string Checker4Status { get; set; }

        public string Checker1 { get; set; }
        public string Checker2 { get; set; }
        public string Checker3 { get; set; }
        public string Checker4 { get; set; }

        public bool IsMakerCompleted { get; set; }
        public string listOfRecommendedSupplier1 { get; set; }
        public string listOfRecommendedSupplier2 { get; set; }
        public string listOfRecommendedSupplier3 { get; set; }
        public string listOfRecommendedSupplier4 { get; set; }
        public int PrincipalFlag { get; set; }
        public int MakerOrHodFlag { get; set; }

        public bool IsChecker1Completed { get; set; }
        public bool IsChecker2Completed { get; set; }
        public bool IsChecker3Completed { get; set; }
        public bool IsChecker4Completed { get; set; }
        public string FacultyName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Photo { get; set; }
        public string FileNames { get; set; }
        public List<string> Files { get; set; }
        public DateTime MakerDate { get; set; }
        public DateTime Checker1Date { get; set; }
        public DateTime Checker2Date { get; set; }
        public DateTime Checker3Date { get; set; }
        public DateTime Checker4Date { get; set; }
    }
}
