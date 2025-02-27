using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;

namespace ActivityManagementSystem.BLL.Common
{
    public class FindContentType
    {
        public static string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(path, out var contentType))
            {
                contentType = "application/octet-stream"; // Default for unknown file types
            }
            return contentType;
        }
    }
}
