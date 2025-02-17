﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class WomenDevelopementResposnse
    {

       
        public string Event { get; set; }
        public string OrganisedBy { get; set; }
        public string ParticipantsFrom { get; set; }
        public string Title { get; set; }
        public string Venue { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Impact { get; set; }
       
        public string InternalOrExternal { get; set; }
        public string StudentOrFaculty { get; set; }
        
        public string OnlineOrOffline { get; set; }
        public List<Student> StudentID { get; set; }
        public List<Faculty> FacultyID { get; set; }
        public List<string> FileBlob { get; set; }

    }
}
