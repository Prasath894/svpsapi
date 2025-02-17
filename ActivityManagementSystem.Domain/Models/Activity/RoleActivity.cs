using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class RoleActivity
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public bool PaperPresentation { get; set; }
        public bool ProjectOrModel { get; set; }
        public bool GuestLectures { get; set; }
        public bool ImplantTrainingOrInternship { get; set; }
        public bool IndustrialVisits { get; set; }
        public bool SportsAndGames { get; set; }
        public bool NCC { get; set; }
        public bool NSS { get; set; }
        public bool FacultyDevelopment { get; set; }
        public bool WomenInDevelopment { get; set; }
        public bool JournalOrBookpublication { get; set; }
        public bool PatentDetails { get; set; }
        public bool SemesterResult { get; set; }
        public bool SymposiumAndExpo { get; set; }
        public bool Placement { get; set; }
        public bool ExtensionServices { get; set; }
        public bool Grants { get; set; }
        public bool AlumniEvent { get; set; }

        public bool Miscellaneous { get; set; }
        public bool Consultancy { get; set; }
        public bool Awards { get; set; }
        public bool Events { get; set; }
        public bool MOUs { get; set; }
        public bool PressReports { get; set; }
        public bool UpcomingEvents { get; set; }
        public bool Advertisements { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedDate { get; set; }
    }
}
