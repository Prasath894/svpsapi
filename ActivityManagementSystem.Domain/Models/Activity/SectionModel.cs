using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class SectionModel
    {

        /// <summary>
        /// Unique identifier for the section.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Grade or class of the section.
        /// </summary>
        [JsonPropertyName("gradeOrClass")]
        public string GradeOrClass { get; set; } = string.Empty;

        /// <summary>
        /// Section name.
        /// </summary>
        [JsonPropertyName("section")]
        public string Section { get; set; } = string.Empty;

        /// <summary>
        /// Coordinator ID for the section.
        /// </summary>
        [JsonPropertyName("coordinatorId")]
        public int CoordinatorId { get; set; } = 0;

        

        /// <summary>
        /// Indicates whether the section is active.
        /// </summary>
        [JsonPropertyName("isActive")]
        public bool? IsActive { get; set; } = true;

        /// <summary>
        /// User who created the record.
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the record was created.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// User who last modified the record.
        /// </summary>
        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// Date and time when the record was last modified.
        /// </summary>
        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }
    }

}
