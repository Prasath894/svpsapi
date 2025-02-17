using System;
namespace ActivityManagementSystem.Domain.AppSettings
{
    public class AppSettings
    {
        public ConnectionInfo ConnectionInfo { get; set; }
        public Settings Settings { get; set; }
        public SmsSettings SmsSettings { get; set; }

        
        public EmailSettings EmailSettings { get; set; }
        public ImageResizerSettings ImageResizerSettings { get; set; }
    }
}
