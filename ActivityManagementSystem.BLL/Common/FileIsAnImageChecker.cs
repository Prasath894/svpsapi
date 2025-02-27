using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.BLL.Common
{
    public class FileIsAnImageChecker
    {
        // Helper method to check if a file is an image
        public static bool IsImageFile(string filePath)
        {
            string[] imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
            string extension = Path.GetExtension(filePath).ToLower();
            return imageExtensions.Contains(extension);
        }
    }
    
}
