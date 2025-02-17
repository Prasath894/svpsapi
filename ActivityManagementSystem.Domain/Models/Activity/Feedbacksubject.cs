using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
    
        public class Feedbacksubject
        {
            /// <summary>
            /// The unique identifier for the subject.
            /// </summary>
            public int SubjectId { get; set; }

            /// <summary>
            /// The unique identifier for the faculty member.
            /// </summary>
            public int FacultyId { get; set; }

            /// <summary>
            /// The name of the faculty member.
            /// </summary>
            public string FacultyName { get; set; }
            public bool IsCompleted { get; set; }
        }
    
}
