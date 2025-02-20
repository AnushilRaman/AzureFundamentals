
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


        public Task<List<string>> GetAllContainerAndBlobs()
        {
            throw new NotImplementedException();
        }
    }
}
