using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.Domain.Models.Activity
{
   
    public class AttachmentModel
    {
        public string FileName { get; set; }   // Name of the image file
        public string FilePath { get; set; }
        public byte[]? BlobData { get; set; }   // Blob (byte array) for the image file
    }
    public class InfoAttachmentModel
    {
        public string FileName { get; set; }   // Name of the image file
        public string FilePath { get; set; }
        public string InfoType { get; set; }
        public byte[]? BlobData { get; set; }   // Blob (byte array) for the image file
    }
}
