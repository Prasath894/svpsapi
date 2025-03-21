using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public  class HolidayCalendar
    {
       /// <summary>
            /// Unique identifier for the student.
            /// </summary>
            [JsonPropertyName("id")]
            public int Id { get; set; }
            /// <summary>
            /// Date of holiday.
            /// </summary>
            [JsonPropertyName("dateofHoliday")]
            public DateTime DateofHoliday { get; set; } = DateTime.MinValue;

            /// <summary>
            /// holiday Details.
            /// </summary>
            [JsonPropertyName("holidayDetails")]
            public string HolidayDetails { get; set; } = string.Empty;
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
            

      }
}
