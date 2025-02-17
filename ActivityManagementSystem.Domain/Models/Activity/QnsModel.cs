using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class QnsModel
    {
        [JsonProperty("QnsId")]
        public long QnsId { get; set; }
        [JsonProperty("QnsDescription")]
        public string QnsDescription { get; set; }
        [JsonProperty("QnsCode")]
        public string QnsCode { get; set; }
        [JsonProperty("Score")]
        public string Score { get; set; }
    }
    public class ScoreModel
    {
        [DisplayName("Question No.")]

        public int QuestionNo { get; set; }

        public string Question { get; set; }
        public long Excellant { get; set; } = 0;
        public long Good { get; set; } = 0;
        public long Medium { get; set; } = 0;
        public long Poor { get; set; } = 0;
        [DisplayName("Very Bad")]
        public long VeryBad { get; set ; } = 0;
        [DisplayName("Marks Secured")]
        public long MarksSecured { get; set; } = 0;
        [DisplayName("Feedback %")]
        public double Feedback { get; set; } = 0;

    }
    public class CourseModel
    {
        [DisplayName("S No.")]
        public long SNo { get; set; }
        public string CourseName { get; set; }
        public string FacultyName { get; set; }
        public double Feedback { get; set; }
        public string LetterGrade { get; set; }
    }
    public class FacultyFeedbackModel
    {
        public string facultyID { get; set; }
        public long Id { get; set; }
        public string facultyName { get; set; }
        public string departmentName { get; set; }
        public long departmentId { get; set; }
    }
}
