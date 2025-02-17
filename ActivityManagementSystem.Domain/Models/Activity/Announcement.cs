using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class Announcement
    {
        public long Id { get; set; }
        public DateTime AnnouncementDate { get; set; }
        //public string Department { get; set; }
        //public string Year { get; set; }
        //public string Semester { get; set; }
        public string Photo { get; set; }
        public string FileNames { get; set; }
        public List<string> Files { get; set; }
        public string SenderType { get; set; }
        public string PhoneNumber { get; set; }
        public bool othersfilecount { get; set; }
        public string MemeberPhoto { get; set; }
        public string MemberFileNames { get; set; }
        public List<string> MemberFiles { get; set; }


        public bool IsEmailSend { get; set; }
        public string EnglishTranslate { get; set; }
        public string TamilTranslate { get; set; }
        public bool IsReadytoSend { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
