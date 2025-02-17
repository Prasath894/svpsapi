using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityManagementSystem.Domain.Models.Activity
{
	public class UserModel
	{
		public long Id { get; set; }
		public long RoleId { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string RoleName { get; set; }
		public string FacultyName { get; set; }
		
	}
}
