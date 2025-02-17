using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BatchListModel
    {
        public int DepartmentId { get; set; }
        public string DeptSection { get; set; }

        public int Year { get; set; }
        public string Sem { get; set; }
    }

    public class Batchdetails
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
       
    }
         
}
