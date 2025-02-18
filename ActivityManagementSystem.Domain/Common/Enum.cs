using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Common
{
    public enum RoleType
    {
        /// <summary>
        /// Role type for student access.
        /// </summary>
        Student,

        /// <summary>
        /// Role type for parent access.
        /// </summary>
        Parent,

        /// <summary>
        /// Role type for faculty access.
        /// </summary>
        Faculty
    }
}
