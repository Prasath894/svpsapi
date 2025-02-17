using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class MOUsResponse
    {
        
        public string NameOfTheIndustryOrOrganization { get; set; }
        public string NatureOfServicesByTheInstitutionOrOrganization { get; set; }
        public string InstitutionOrProgramme { get; set; }
        public string MOUDate { get; set; }    
        public string MOUStartDate { get; set; }
        public string MOUEndDate { get; set; }

        
       
        public List<string> FileBlob { get; set; }

    }
}
