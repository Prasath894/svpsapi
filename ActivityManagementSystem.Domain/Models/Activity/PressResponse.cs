using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class PressResponse
    {
        public string NameOfNewspaper { get; set; }
        public string NewspaperEdition { get; set; }
        public string DateOfPublished { get; set; }
        public string InstitutionProgrammesOrCIICP { get; set; }
        public string NewspaperPage { get; set; }
        public string PressReportsType { get; set; }
        public string DescriptionOfFunction { get; set; }      
        public string DescriptionOfAward { get; set; }
        public string DescriptionOfEvent { get; set; }
        public string DescriptionOfOthers { get; set; }       
       
        public List<string> FileBlob { get; set; }
    }
}
