using System;
namespace ActivityManagementSystem.Domain.AppSettings
{
    public class Settings
    {
        public string UiUrl { get; set; }
        public int SecurityTimeStamp { get; set; }
        public bool IsNonceEnabled { get; set; }
        public bool IsEtagEnabled { get; set; }
        public bool IsCacheEnabled { get; set; }
        public bool IsLoggingEnabled { get; set; }
        public bool IsTestModeEnabled { get; set; }
        public string ServerTimezone { get; set; }
        public string Path { get; set; }
        public string AuthUri { get; set; }

        public string DownloadPath { get; set; }
        public string UploadFilePath { get; set; }

        public string FileName { get; set; }
        public string FileNames { get; set; }
        public string PdfFile { get; set; }

    }
}
