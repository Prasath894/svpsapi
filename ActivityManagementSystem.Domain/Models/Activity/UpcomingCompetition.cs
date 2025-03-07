using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class UpcomingCompetition
    {
        /// <summary>
        /// Unique identifier for the competition.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Grade level for which the competition is organized.
        /// </summary>
        [JsonPropertyName("grade")]
        public string Grade { get; set; } = string.Empty;

        /// <summary>
        /// Date of the event.
        /// </summary>
        [JsonPropertyName("eventDate")]
        public DateTime EventDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Day of the event.
        /// </summary>
        [JsonPropertyName("eventDay")]
        public string EventDay { get; set; } = string.Empty;

        /// <summary>
        /// Timing details of the event.
        /// </summary>
        [JsonPropertyName("eventTiming")]
        public string EventTiming { get; set; } = string.Empty;

        /// <summary>
        /// NAme of the event.
        /// </summary>
        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = string.Empty;
        /// <summary>
        /// Eligibility criteria for the event.
        /// </summary>
        [JsonPropertyName("eligibility")]
        public string Eligibility { get; set; } = string.Empty;

        /// <summary>
        /// Guidelines for participation.
        /// </summary>
        [JsonPropertyName("guidelines")]
        public string Guidelines { get; set; } = string.Empty;

        /// <summary>
        /// Time limit for the competition.
        /// </summary>
        [JsonPropertyName("timeLimit")]
        public string TimeLimit { get; set; } = string.Empty;

        /// <summary>
        /// Criteria for judging the competition.
        /// </summary>
        [JsonPropertyName("judgingCriteria")]
        public string JudgingCriteria { get; set; } = string.Empty;

        /// <summary>
        /// Dress code required for the event.
        /// </summary>
        [JsonPropertyName("dressCode")]
        public string DressCode { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether polling is required for the competition.
        /// </summary>
        [JsonPropertyName("isPollingRequired")]
        public bool IsPollingRequired { get; set; } = false;

        /// <summary>
        /// End date for polling.
        /// </summary>
        [JsonPropertyName("pollingEndDate")]
        public DateTime? PollingEndDate { get; set; }
        public string FileNames { get; set; }
        public string FilePath { get; set; }  // varchar(500)

        public List<string> Files { get; set; }

        /// <summary>
        /// User who created the record.
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of record creation.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User who last modified the record.
        /// </summary>
        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp of the last modification.
        /// </summary>
        [JsonPropertyName("modifiedDate")]
        public DateTime? ModifiedDate { get; set; }
    }
}
