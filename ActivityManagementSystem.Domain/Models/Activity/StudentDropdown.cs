using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StudentDropdown
    {

        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Section { get; set; }
        public string GradeOrClass { get; set; }

        public int SectionId { get; set; }
    }
}
