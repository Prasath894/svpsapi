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

}
