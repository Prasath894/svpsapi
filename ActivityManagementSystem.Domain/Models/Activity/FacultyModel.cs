using System;
using System.Collections.Generic;
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
        public int DepartmentId { get; set; } = 0;
        public string Department { get; set; } = string.Empty;
        public string FacultyMobileNo_1 { get; set; } = string.Empty;
        public string FacultyMobileNo_2 { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string IndentRoleName { get; set; } = string.Empty;
        public string FdpRoleName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }=DateTime.UtcNow;
        public string FileNames { get; set; } = string.Empty;
        public List<string>? Files { get; set; }
    }
}
