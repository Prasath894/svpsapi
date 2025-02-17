using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AdvertisementsResponse
    {
        public string NameOfNewspaper { get; set; }
        public string NewspaperEdition { get; set; }
        public string NewspaperPage { get; set; }
        public string DateOfPublished { get; set; }
        public string AdvertisementType { get; set; }
        public string DescriptionOfAdmissions { get; set; }
        public string DescriptionofWanted { get; set; }
        public string DescriptionOfEvent { get; set; }
      
        public string InstitutionProgrammesOrCIICP { get; set; }
        
        public List<string> FileBlob { get; set; }
    }
}
