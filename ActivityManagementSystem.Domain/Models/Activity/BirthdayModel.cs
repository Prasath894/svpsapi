using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class BirthdayModel
    {
        // <summary>
        /// Gets or sets the unique identifier of the person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the person.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the department name of the person.
        /// </summary>
        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the year of the person.
        /// </summary>
        [JsonPropertyName("year")]
        public string Year { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the role of the person.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the gender of the person.
        /// </summary>
        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;
    }
}
