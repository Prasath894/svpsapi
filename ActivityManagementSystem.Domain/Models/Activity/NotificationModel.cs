using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class NotificationModel
    {
        /// <summary>
        /// Gets or sets the type of the message or notification.
        /// </summary>
        [JsonPropertyName("msgType")]
        public string MsgType { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the student associated with the notification.
        /// </summary>
        [JsonPropertyName("studentId")]
        public long StudentId { get; set; }

        /// <summary>
        /// Gets or sets the notification message content.
        /// </summary>
        [JsonPropertyName("notificationMsg")]
        public string NotificationMsg { get; set; }
    }
    public class UserFcmToken
    {
        public int UserId { get; set; }
        public string Role { get; set; }
        public string DeviceToken { get; set; }
    }
}
