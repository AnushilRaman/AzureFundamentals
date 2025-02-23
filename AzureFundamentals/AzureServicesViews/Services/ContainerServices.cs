
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureServicesViews.Services
{
    public class ContainerServices : IContainerServices
    {
        private readonly BlobServiceClient _blobservicesClient;

        public ContainerServices(BlobServiceClient blobservicesClient)
        {
            _blobservicesClient = blobservicesClient;
        }

        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobservicesClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

        }

        public async Task DeleteContainer(string containerName)
        {
            await _blobservicesClient.DeleteBlobContainerAsync(containerName);
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerNames = new();
            await foreach (BlobContainerItem blobContainerItem in _blobservicesClient.GetBlobContainersAsync())
            {
                containerNames.Add(blobContainerItem.Name);
            }
            return containerNames;
        }


        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobNames = new();
            containerAndBlobNames.Add("Account Name : " + _blobservicesClient.AccountName);
            containerAndBlobNames.Add("--------------------------------------------");
            await foreach (BlobContainerItem blobContainerItem in _blobservicesClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("-----" + blobContainerItem.Name);
                BlobContainerClient blobContainerClient = _blobservicesClient.GetBlobContainerClient(blobContainerItem.Name);
                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
                {
                    //get metadata
                    var blobClient = blobContainerClient.GetBlobClient(blobItem.Name);
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    string blobToAdd = blobItem.Name;
                    if (blobProperties.Metadata.ContainsKey("title"))
                        blobToAdd += "(" + blobProperties.Metadata["title"] + ")";
                    containerAndBlobNames.Add("----" + blobToAdd);
                }
                containerAndBlobNames.Add("--------------------------------------------");
            }
            return containerAndBlobNames;
        }
    }
}
