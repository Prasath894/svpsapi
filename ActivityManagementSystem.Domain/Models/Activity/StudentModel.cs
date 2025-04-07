using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StudentModel
    {
        /// <summary>
        /// Unique identifier for the student.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Admission number of the student.
        /// </summary>
        [JsonPropertyName("admissionNumber")]
        public string AdmissionNumber { get; set; } = string.Empty;



        /// <summary>
        /// First name of the student.
        /// </summary>
        [JsonPropertyName("student_FirstName")]
        public string Student_FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Middle name of the student.
        /// </summary>
        [JsonPropertyName("student_MiddleName")]
        public string Student_MiddleName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the student.
        /// </summary>
        [JsonPropertyName("student_LastName")]
        public string Student_LastName { get; set; } = string.Empty;


        /// <summary>
        /// Section ID of the student.
        /// </summary>
        [JsonPropertyName("sectionId")]
        public int SectionId { get; set; } = 0;

        /// <summary>
        /// Allergies information.
        /// </summary>
        [JsonPropertyName("allergicTo")]
        public string AllergicTo { get; set; } = string.Empty;

        /// <summary>
        /// House ID of the student.
        /// </summary>
        [JsonPropertyName("houseId")]
        public int HouseId { get; set; } = 0;
        /// <summary>
        /// Role ID of the student.
        /// </summary>
        [JsonPropertyName("roleId")]
        public int RoleId { get; set; } = 0;

        /// <summary>
        /// Blood group of the student.
        /// </summary>
        [JsonPropertyName("bloodGroup")]
        public string BloodGroup { get; set; } = string.Empty;

        /// <summary>
        /// Gender of the student.
        /// </summary>
        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Date of birth.
        /// </summary>
        [JsonPropertyName("dob")]
        public DateTime Dob { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Communication address.
        /// </summary>
        [JsonPropertyName("communicationAddress")]
        public string CommunicationAddress { get; set; } = string.Empty;

        /// <summary>
        /// Permanent address.
        /// </summary>
        [JsonPropertyName("permanentAddress")]
        public string PermanentAddress { get; set; } = string.Empty;

        /// <summary>
        /// Aadhaar number of the student.
        /// </summary>
        [JsonPropertyName("student_AadhaarNumber")]
        public string Student_AadhaarNumber { get; set; } = string.Empty;

        /// <summary>
        /// Parent's email ID.
        /// </summary>
        [JsonPropertyName("parentEmailId")]
        public string ParentEmailId { get; set; } = string.Empty;

        /// <summary>
        /// Student's photo file path.
        /// </summary>
        [JsonPropertyName("photo")]
        public string Photo { get; set; } = string.Empty;

        /// <summary>
        /// Father's name.
        /// </summary>
        [JsonPropertyName("fatherName")]
        public string FatherName { get; set; } = string.Empty;

        /// <summary>
        /// Father's mobile number.
        /// </summary>
        [JsonPropertyName("father_MobileNumber")]
        public string Father_MobileNumber { get; set; } = string.Empty;



        /// <summary>
        /// Father's photo.
        /// </summary>
        [JsonPropertyName("father_Photo")]
        public string Father_Photo { get; set; } = string.Empty;

        /// <summary>
        /// Mother's name.
        /// </summary>
        [JsonPropertyName("motherName")]
        public string MotherName { get; set; } = string.Empty;

        /// <summary>
        /// Mother's mobile number.
        /// </summary>
        [JsonPropertyName("mother_MobileNumber")]
        public string Mother_MobileNumber { get; set; } = string.Empty;



        /// <summary>
        /// Mother's photo.
        /// </summary>
        [JsonPropertyName("mother_Photo")]
        public string Mother_Photo { get; set; } = string.Empty;

        /// <summary>
        /// Guardian 1 name.
        /// </summary>
        [JsonPropertyName("gardian1Name")]
        public string Gardian1Name { get; set; } = string.Empty;

        /// <summary>
        /// Guardian 1 mobile number.
        /// </summary>
        [JsonPropertyName("gardian1MobileNumber")]
        public string Gardian1MobileNumber { get; set; } = string.Empty;



        /// <summary>
        /// Guardian 1 photo.
        /// </summary>
        [JsonPropertyName("gardian1Photo")]
        public string Gardian1Photo { get; set; } = string.Empty;

        /// <summary>
        /// Guardian 2 name.
        /// </summary>
        [JsonPropertyName("gardian2Name")]
        public string Gardian2Name { get; set; } = string.Empty;

        /// <summary>
        /// Guardian 2 mobile number.
        /// </summary>
        [JsonPropertyName("gardian2MobileNumber")]
        public string Gardian2MobileNumber { get; set; } = string.Empty;



        /// <summary>
        /// Guardian 2 photo.
        /// </summary>
        [JsonPropertyName("gardian2Photo")]
        public string Gardian2Photo { get; set; } = string.Empty;



        /// <summary>
        /// Guardian 3 name.
        /// </summary>
        [JsonPropertyName("gardian3Name")]
        public string Gardian3Name { get; set; } = string.Empty;

        /// <summary>
        /// Guardian 3 mobile number.
        /// </summary>
        [JsonPropertyName("gardian3MobileNumber")]
        public string Gardian3MobileNumber { get; set; } = string.Empty;



        /// <summary>
        /// Guardian 3 photo.
        /// </summary>
        [JsonPropertyName("gardian3Photo")]
        public string Gardian3Photo { get; set; } = string.Empty;


        /// <summary>
        /// Guardian 4 name.
        /// </summary>
        [JsonPropertyName("gardian4Name")]
        public string Gardian4Name { get; set; } = string.Empty;

        /// <summary>
        /// Guardian 4 mobile number.
        /// </summary>
        [JsonPropertyName("gardian4MobileNumber")]
        public string Gardian4MobileNumber { get; set; } = string.Empty;



        /// <summary>
        /// Guardian 4 photo.
        /// </summary>
        [JsonPropertyName("gardian4Photo")]
        public string Gardian4Photo { get; set; } = string.Empty;




        /// <summary>
        /// Section mapping status.
        /// </summary>
        [JsonPropertyName("isSectionMapped")]
        public bool? IsSectionMapped { get; set; } = false;

        /// <summary>
        /// File path.
        /// </summary>
        [JsonPropertyName("filePath")]
        public string FilePath { get; set; } = string.Empty;
        /// <summary>
        /// File name.
        /// </summary>
        [JsonPropertyName("fileNames")]
        public string FileNames { get; set; } = string.Empty;
        /// <summary>
        /// Files.
        /// </summary>
        [JsonPropertyName("files")]
        public List<string> Files { get; set; }

        /// <summary>
        /// Active status.
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; } = true;

        /// <summary>
        /// User who created the record.
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Record creation date.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// User who modified the record.
        /// </summary>
        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// Record modification date.
        /// </summary>
        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }
        /// <summary>
        /// Full name of the student (combining first, middle, and last names).
        /// </summary>
        [JsonPropertyName("studentName")]
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Name of the house assigned to the student.
        /// </summary>
        [JsonPropertyName("houseName")]
        public string HouseName { get; set; } = string.Empty;

    }
}
