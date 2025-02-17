using System;
namespace ActivityManagementSystem.Domain.AppSettings
{
    public class EmailSettings
    {
        public string ApiKey { get; set; }
        public string GuardianResetPasswordLink { get; set; }
        public string PatientResetPasswordLink { get; set; }
        public string GotoDashBoardLink { get; set; }
        public string EnrollEmailAddress { get; set; }
        public string GoToApplication { get; set; }

    }
}
