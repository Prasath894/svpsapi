using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class AcademicCalender
    {
        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        [JsonPropertyName("sNo")]
        public int SNo { get; set; }

        /// <summary>
        /// Gets or sets the academic activity description.
        /// </summary>
        [JsonPropertyName("academicActivities")]
        public string AcademicActivities { get; set; } = "Reopening of the Institution";

        /// <summary>
        /// Gets or sets the start date of the academic activity.
        /// </summary>
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; } = new DateTime(2025, 3, 1);

        /// <summary>
        /// Gets or sets the end date of the academic activity.
        /// </summary>
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; } = null;

        /// <summary>
        /// Gets or sets the name of the user who created the record.
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = "Admin";

        /// <summary>
        /// Gets or sets the date and time when the record was created.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Parse("2025-01-23T10:32:52.360");

        /// <summary>
        /// Gets or sets the name of the user who last modified the record.
        /// </summary>
        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; } = null;

        /// <summary>
        /// Gets or sets the date and time when the record was last modified.
        /// </summary>
        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; } = null;
    }
}
