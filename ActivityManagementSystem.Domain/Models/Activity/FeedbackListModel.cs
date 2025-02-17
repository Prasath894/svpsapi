using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class FeedbackListModel
    {

       
            public string StudentId { get; set; }
            public string StudentName { get; set; }           
            public string FeedbackEndDate { get; set; }
            public string Father_MobileNumber { get; set; }


        
    }

    public class FormFacultyNoModel
    {
        public string WhatsAppNum { get; set; }
        public string ProgrammeName { get; set; }
        public string FacultyName { get; set; }
        public string TitleOfTheProgramme { get; set; }
        public string NameOfTheLaboratory { get; set; }
        public string FormDate { get; set; }
        public string Purpose { get; set; }
        public string Message { get; set; }
        public string senderName { get; set; }        


    }
}
