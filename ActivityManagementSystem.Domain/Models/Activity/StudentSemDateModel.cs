using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StudentSemDateModel
    {
        //public long Id { get; set; }
        public string Sem { get; set; }
        public string FirstYearStartDate { get; set; }
        public string FirstYearEndDate { get; set; }
        public string SecondYearStartDate { get; set; }
        public string SecondYearEndDate { get; set; }
        public string ThirdYearStartDate { get; set; }
        public string ThirdYearEndDate { get; set; }
        public string FeedbackStartDate { get; set; }
        public string FeedbackEndDate { get; set; }
       // public long Department { get; set; }
       // public bool IsFeedbackSend { get; set; }
       // public string CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

    }
}
