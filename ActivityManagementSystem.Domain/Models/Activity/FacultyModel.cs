using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model representing faculty information.
    /// </summary>
    public class FacultyModel
    {
        /// <summary>Faculty DB Id</summary>
        [JsonPropertyName("id")]
        public long Id { get; set; } = 0;

        /// <summary>Username for login</summary>
        [JsonPropertyName("userName")]
        public string UserName { get; set; } = string.Empty;

        /// <summary>Login password</summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>Role Id</summary>
        [JsonPropertyName("roleId")]
        public long RoleId { get; set; } = 0;

        /// <summary>Role name</summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>Faculty identifier</summary>
        [JsonPropertyName("facultyId")]
        public string FacultyId { get; set; } = string.Empty;

        [JsonPropertyName("facultyName")]
        public string FacultyName { get; set; } = string.Empty;

        [JsonPropertyName("faculty_FirstName")]
        public string Faculty_FirstName { get; set; } = string.Empty;

        [JsonPropertyName("faculty_MiddleName")]
        public string? Faculty_MiddleName { get; set; } = string.Empty;

        [JsonPropertyName("faculty_LastName")]
        public string Faculty_LastName { get; set; } = string.Empty;

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("dob")]
        public DateTime DOB { get; set; } = DateTime.Now;

        [JsonPropertyName("facultyMobileNo_1")]
        public string FacultyMobileNo_1 { get; set; } = string.Empty;

        [JsonPropertyName("facultyMobileNo_2")]
        public string FacultyMobileNo_2 { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;

        [JsonPropertyName("fileNames")]
        public string FileNames { get; set; } = string.Empty;

        [JsonPropertyName("bloodGroup")]
        public string BloodGroup { get; set; } = string.Empty;

        [JsonPropertyName("files")]
        public List<string> files { get; set; } = new List<string>();

        [JsonPropertyName("address")]
        public string Address { get; set; } = string.Empty;

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; } = string.Empty;

        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; } = DateTime.UtcNow;
    }

}
