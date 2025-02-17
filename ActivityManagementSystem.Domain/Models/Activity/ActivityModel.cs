using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class ActivityModel
    {
        public long Id { get; set; }

        public long DepartmentID { get; set; }
        public string FileNames { get; set; }
        public long ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string Data { get; set; }
        public string FilePath { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }

        

        public List<string> Files { get; set; }

        public List<string> FileBlob { get; set; }
    }
}
