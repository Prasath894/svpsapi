using ActivityManagementSystem.Domain.Models.Activity;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Erp.Application.Common
{
    public static class FileOperations
    {
        /// <summary>
        /// To save the uploaded file as timestamp as the file name
        /// </summary>
        /// <param name="fileUpload">The file upload control</param>
        /// <returns>The saved file path</returns>
        public static async Task<(string, string)> SaveFileWithTimeStamp(ExcelUpload fileUpload)
        {
            if (fileUpload.FormFiles != null)
            {
             
                var target = Path.Combine(Directory.GetCurrentDirectory(), fileUpload.TypeofUser.ToString());
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                }

                var fileName = $"{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(fileUpload.FormFiles.FileName)}";

                var filePath = Path.Combine(target, fileName);
                using Stream stream = new FileStream(filePath, FileMode.Create);
                await fileUpload.FormFiles.CopyToAsync(stream);
                return (filePath, fileName);
            }
            return (string.Empty, string.Empty);
        }
        public static string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".txt":
                    return "text/plain";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".pdf":
                    return "application/pdf";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".csv":
                    return "text/csv";
                case ".xls":
                case ".xlsx":
                    return "application/vnd.ms-excel";
                case ".pptx":
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                // Add more cases for other file types if needed
                default:
                    return null; // unsupported file type
            }
        }
    }
}
