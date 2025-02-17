using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
   public class JournalResponse
    {
        public string JournalPaperTitle { get; set; }
        public string JournalName { get; set; }
        public string JournalPublisher { get; set; }
        public long TotalJournalPages { get; set; }
        public string JournalType { get; set; }
        public string JournalIssueDate { get; set; }
       
        public string JournalAbstract { get; set; }
        public string Department { get; set; }
        public string BookName { get; set; }
        public string BookPublishingOn { get; set; }
        public string BookAuthorName { get; set; }

        public string BookCoAuthor { get; set; }

        public string BookPublisherName { get; set; }

        public string BookVolumeNo { get; set; }

        public string BookType { get; set; }
        public string BookPublicationNumber { get; set; }

        public string BookIssueDate { get; set; }      

        public string BookRemarks { get; set; }
  
        public string StudentOrFaculty { get; set; }

        
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }

        public List<string> FileBlob { get; set; }

    }

   

}


