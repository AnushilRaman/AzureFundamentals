
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureServicesViews.Models;

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

        public async Task<bool> UploadBlob(string name, IFormFile formFile, string containerName, Blob blob)
        {
            blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = formFile.ContentType,
            };
            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("title", blob.Title);
            metaData["comment"] = blob.Comment;

            var result = await blobClient.UploadAsync(formFile.OpenReadStream(), httpHeaders, metaData);
            if (result == null) return false;
            return true;
        }
    }
}
