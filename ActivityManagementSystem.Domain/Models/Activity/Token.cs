using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class Token
    {
        /// <summary>
        /// The token to be used for accessing the resources.
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// The token expiry time.
        /// </summary>
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The Name of the login user.
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// The Name of the login user.
        /// </summary>
        public string FacultyUsername { get; set; } = string.Empty;

        /// <summary>
        /// The Role Name of the login user.
        /// </summary>
        public string UserRole { get; set; } = string.Empty;

        /// <summary>
        /// The Id of the user.
        /// </summary>
        public int UserId { get; set; } = 0;
    }
}
