using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AlumniModel
    {
        public long Id { get; set; }
        public string AlumniId { get; set; }
        public string AlumniFirstName { get; set; }
        public string AlumniMiddleName { get; set; }
        public string AlumniLastName { get; set; }
        public string PassedOutYear { get; set; }
        public int DepartmentId { get; set; }
        public string Gender { get; set; }
        public string AlumniMobileNumber_1 { get; set; }
        public string AlumniMobileNumber_2 { get; set; }
        public string Email { get; set; }
        public string Photo { get; set; }
        public string Profession { get; set; }
        public string CurrentDesignation { get; set; }
        public string CompanyNameOrOrganisation { get; set; }
        public string AddressOfCompanyOrOrganisation { get; set; }
        public string CurrentWorkingPlace { get; set; }
        public string CommunicationAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string ContributionTowardsInstitution { get; set; }
        public string FundContribution { get; set; }
        public string PanNumber { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string FileNames { get; set; }

        public List<string> Files { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
