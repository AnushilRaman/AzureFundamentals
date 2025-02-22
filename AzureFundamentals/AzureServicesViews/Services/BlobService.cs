﻿
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureServicesViews.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;
        BlobContainerClient blobContainerClient = null;
        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var result = await blobContainerClient.DeleteBlobIfExistsAsync(name);
            if (result != null) { return true; }
            return false;
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            var blobString = new List<string>();
            await foreach (var item in blobs)
            {
                blobString.Add(item.Name);
            }
            return blobString;
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
            blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string name, IFormFile formFile, string containerName)
        {
            blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = formFile.ContentType,
            };
            var result = await blobClient.UploadAsync(formFile.OpenReadStream(), httpHeaders);
            if (result == null) return false;
            return true;
        }
    }
}
