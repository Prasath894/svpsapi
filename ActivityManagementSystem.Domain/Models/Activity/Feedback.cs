using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class Feedback
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string RollNo { get; set; }
        public string StudentName { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public int facultyId { get; set; }
        public string facultyReg { get; set; }
        public string facultyName { get; set; }
        public int SubjectId { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsCompleted { get; set; }

        public string Sem { get; set; }
        public string Year { get; set; }
        public string FeedbackData { get; set; }
        public string Section { get; set; }
        public string FeedbackReviewScore { get; set; }
        public string StudentNote { get; set; }

        public List<QnsModel> Questions { get; set; }
    }
}
