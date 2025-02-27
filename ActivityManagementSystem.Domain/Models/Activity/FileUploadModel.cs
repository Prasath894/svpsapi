using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class FileUploadModel
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public int ActivityID { get; set; }
        public string ActivityName { get; set; }
        public List<IFormFile> FormFiles { get; set; }
    }

    
    public class FileUpload
    {
        public int Id { get; set; }
        public string TypeofUser { get; set; }        

        public List<IFormFile> FormFiles { get; set; }
    }
    public class FileAnnouncementUpload
    {
        public int Id { get; set; }
        public DateTime AnnouncementDate { get; set; }
        //public string Department { get; set; }
        //public string Year { get; set; }
        //public string Semester { get; set; }
        public string Filepath { get; set; }
        public string FileNames { get; set; }
        public List<string> Files { get; set; }
        public string SenderType { get; set; }


        public bool IsEmailSend { get; set; }
        public string EnglishTranslate { get; set; }
        public string TamilTranslate { get; set; }
        public bool IsReadytoSend { get; set; }
        public string TypeofUser { get; set; }

        public List<IFormFile> FormFiles { get; set; }
    }

}
