using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class QuatationModel
    {
        public long RefNo { get; set; }
        public int principalFlag { get; set; }        
        public string currentFileQueueStatus { get; set; }
        public bool IsMakerCompleted { get; set; }
        public bool IsChecker1Completed { get; set; }
        public bool IsChecker2Completed { get; set; }
        public bool IsChecker3Completed { get; set; }
        public bool IsChecker4Completed { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
