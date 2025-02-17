using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class PlacementResponse
    {

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyType { get; set; }
        public string Department { get; set; }
        public string SalaryPerAnnum { get; set; }
        public string NStudentAttended { get; set; }
        public string NStudentAttendedBoys { get; set; }
        public string NStudentAttendedGirls { get; set; }
        public string NStudentPlaced { get; set; }
        public string NStudentPlacedBoys { get; set; }
        public string NStudentPlacedGirls { get; set; }
        public string FromDate { get; set; }
     
        public string OffcampusOrOncampus { get; set; }
     
       
        public List<string> FileBlob { get; set; }

    }
}
