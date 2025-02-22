using AzureServicesViews.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureServicesViews.Controllers
{
    public class BlobController : Controller
    {
        private readonly IBlobService _blobService;

        public BlobController(IBlobService blobService)
        {
            _blobService = blobService;
        }

        public async Task<IActionResult> Manage(string containerName)
        {
            var blobs = await _blobService.GetAllBlobs(containerName);
            return View(blobs);
        }

        [HttpGet]
        public IActionResult AddFile(string containerName)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddFile(string containerName, IFormFile formFile)
        {
            if (formFile == null || formFile.Length < 1)
            {
                return View();
            }
            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(formFile.FileName);
            var result = await _blobService.UploadBlob(fileName, formFile, containerName);
            if (result)
                return RedirectToAction("Index", "Container");
            return View();
        }
    }
}
