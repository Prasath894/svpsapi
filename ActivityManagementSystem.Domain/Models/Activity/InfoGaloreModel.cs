using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class InfoGaloreModel
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the info file.
        /// </summary>
        [JsonPropertyName("infoFileName")]
        public string InfoFileName { get; set; }
        /// <summary>
        /// Gets or sets the type of the info file.
        /// </summary>
        [JsonPropertyName("infoType")]
        public string InfoType { get; set; }

        /// <summary>
        /// Gets or sets the path of the info file.
        /// </summary>
        [JsonPropertyName("infoFilePath")]
        public string InfoFilePath { get; set; }
        /// <summary>
        /// Gets or sets the file of the info galore.
        /// </summary>
        [JsonPropertyName("infoFile")]
        public List<IFormFile> InfoFile { get; set; }

        /// <summary>
        /// Gets or sets the valid date.
        /// </summary>
        [JsonPropertyName("validDate")]
        public DateTime ValidDate { get; set; }


        /// <summary>
        /// Gets or sets the user who created the record.
        /// </summary>
        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was created.
        /// </summary>
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
    }
}
