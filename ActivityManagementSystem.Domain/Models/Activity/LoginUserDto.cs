using ActivityManagementSystem.Domain.Common;
using System.Text.Json.Serialization;

namespace ActivityManagementSystem.Domain.Models
{ 
    /// <summary>
    /// The logging-in user details.
    /// </summary>
    public class LoginUserDto
    {
        /// <summary>
        /// The mobile no of the logging-in user.
        /// </summary>
        [JsonPropertyName("admissionNo")]
        public string AdmissionNo { get; set; } = string.Empty;
        /// <summary>
        /// The Username of the logging-in user.
        /// </summary>
        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// The Password of the logging-in user.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// The Role of the logging-in user.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; }
    
    }
}
