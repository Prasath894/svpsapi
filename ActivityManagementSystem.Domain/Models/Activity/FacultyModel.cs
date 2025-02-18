using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class FacultyModel
    {
        public long Id { get; set; } = 0;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public long RoleId { get; set; } = 0;
        public string Role { get; set; } = string.Empty;

        public string FacultyId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string Faculty_FirstName { get; set; } = string.Empty;
        public string? Faculty_MiddleName { get; set; } = string.Empty;
        public string Faculty_LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DOB { get; set; }=DateTime.Now;
        public string FacultyMobileNo_1 { get; set; } = string.Empty;
        public string FacultyMobileNo_2 { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
       
        public string FilePath { get; set; } = string.Empty;
        public string FileNames { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;

        public List<string> files { get; set; }

        public string Address { get; set; } = string.Empty;
        
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }=DateTime.UtcNow;
       
    }
}
