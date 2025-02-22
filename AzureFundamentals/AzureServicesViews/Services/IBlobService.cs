namespace AzureServicesViews.Services
{
    public interface IBlobService
    {
        Task<string> GetBlob(string name, string containerName);
        Task<List<string>> GetAllBlobs(string containerName);
        Task<bool> UploadBlob(string name, IFormFile formFile, string containerName);
        Task<bool> DeleteBlob(string name, string containerName);
    }
}
