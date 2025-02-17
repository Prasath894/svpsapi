using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ExcelUpload
    {
        public string TypeofUser { get; set; }
        public string Sem { get; set; }
        public string Section { get; set; }
        public string Year { get; set; }
        public long department { get; set; }

        
        public bool isAttendnce { get; set; }
        public IFormFile FormFiles { get; set; }
    }
}
