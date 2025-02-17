using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BatchStudentMapping
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string ClassSection { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public string Sem { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int SubjectId { get; set; }
        public string Subject { get; set; }

    }

    public class BatchStudentSubjectList
    {
        public int BatchId { get; set; }
        public string BatchName { get; set; }
        public string ClassSection { get; set; }
        public string Department { get; set; }
        public int Year { get; set; }
        public string Sem { get; set; }
        public List<StudentMap> Students {get;set;}
        public List<SubjectMap> Subjects { get; set; }
    }
    public class StudentMap
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
    }
    public class SubjectMap
    {
        public int SubjectId { get; set; }
        public string Subject { get; set; }
    }
}
