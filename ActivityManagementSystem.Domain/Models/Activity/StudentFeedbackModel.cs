using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    public class StudentFeedbackModel
    {     
            /// <summary>
            /// Gets or sets the unique identifier for the feedback.
            /// </summary>
            [JsonPropertyName("id")]
            public int Id { get; set; }

            /// <summary>
            /// Gets or sets the identifier for the faculty associated with the feedback.
            /// </summary>
            [JsonPropertyName("facultyId")]
            public int FacultyId { get; set; }
            /// <summary>
            /// Gets or sets the name for the faculty associated with the feedback.
            /// </summary>
            [JsonPropertyName("facultyName")]
            public string FacultyName { get; set; }
            /// <summary>
            /// Gets or sets the name for the student associated with the feedback.
            /// </summary>
            [JsonPropertyName("studentName")]
            public string StudentName { get; set; }
            /// <summary>
            /// Gets or sets the rollno for the student associated with the feedback.
            /// </summary>
            [JsonPropertyName("studentRollNo")]
            public string StudentRollNo { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the student providing the feedback.
        /// </summary>
        [JsonPropertyName("studentId")]
            public int StudentId { get; set; }

            /// <summary>
            /// Gets or sets the feedback content.
            /// </summary>
            [JsonPropertyName("feedback")]
            public string Feedback { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the feedback has been sent.
            /// </summary>
            [JsonPropertyName("isReadyToSentWhatsapp")]
            public bool IsReadyToSentWhatsapp { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether the feedback has been sent.
            /// </summary>
            [JsonPropertyName("isSentAsWhatsapp")]
            public bool IsSentAsWhatsapp { get; set; }

            /// <summary>
            /// Gets or sets the name of the user who created the feedback record.
            /// </summary>
            [JsonPropertyName("createdBy")]
            public string CreatedBy { get; set; }

            /// <summary>
            /// Gets or sets the date and time when the feedback record was created.
            /// </summary>
            [JsonPropertyName("createdDate")]
            public DateTime CreatedDate { get; set; }

            /// <summary>
            /// Gets or sets the name of the user who last modified the feedback record.
            /// </summary>
            [JsonPropertyName("modifiedBy")]
            public string ModifiedBy { get; set; }

            /// <summary>
            /// Gets or sets the date and time when the feedback record was last modified.
            /// </summary>
            [JsonPropertyName("modifiedDate")]
            public DateTime ModifiedDate { get; set; }

            public string FileName { get; set; }  // varchar(100)
            public string FilePath { get; set; }  // varchar(100)

            public List<string>? FileList { get; set; }
    }
    }
