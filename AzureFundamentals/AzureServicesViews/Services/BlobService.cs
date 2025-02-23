
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
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

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            var blobList = new List<Blob>();
            var sasContainerSignature = "";
            //Container Level Sas Token generation
            if (blobContainerClient.CanGenerateSasUri)
            {
                BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTime.UtcNow.AddHours(1)
                };
                blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
                sasContainerSignature = blobContainerClient.GenerateSasUri(blobSasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }

            await foreach (var item in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(item.Name);
                Blob blobIndividual = new()
                {
                    Uri = blobClient.Uri.AbsoluteUri + "?" + sasContainerSignature
                };

                //SAS Token at Blob File level
                //if (blobClient.CanGenerateSasUri)
                //{
                //    BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                //    {
                //        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                //        BlobName = item.Name,
                //        Resource = "b",
                //        ExpiresOn = DateTime.UtcNow.AddHours(1)
                //    };
                //    blobSasBuilder.SetPermissions(BlobSasPermissions.Read);
                //    blobIndividual.Uri = blobClient.GenerateSasUri(blobSasBuilder).AbsoluteUri;
                //}
                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                if (blobProperties.Metadata.ContainsKey("title"))
                    blobIndividual.Title = blobProperties.Metadata["title"];
                if (blobProperties.Metadata.ContainsKey("comment"))
                    blobIndividual.Comment = blobProperties.Metadata["comment"];
                blobList.Add(blobIndividual);
            }
            return blobList;
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
