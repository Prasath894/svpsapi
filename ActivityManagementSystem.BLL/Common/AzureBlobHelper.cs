using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityManagementSystem.BLL.Common
{
    public class AzureBlobHelper
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobHelper(IConfiguration config)
        {
            var connectionString = config["AzureBlobStorage:ConnectionString"];
            var containerName = config["AzureBlobStorage:ContainerName"];
            _containerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder)
        {
            await _containerClient.CreateIfNotExistsAsync();
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobPath = $"{folder}/{fileName}";

            var blobClient = _containerClient.GetBlobClient(blobPath);
            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString(); // You can store this URI in DB
        }

        public async Task<Stream> DownloadFileAsync(string folder, string fileName)
        {
            var blobPath = $"{folder}/{fileName}";
            var blobClient = _containerClient.GetBlobClient(blobPath);
            var response = await blobClient.DownloadAsync();
            return response.Value.Content;
        }
    }

}
