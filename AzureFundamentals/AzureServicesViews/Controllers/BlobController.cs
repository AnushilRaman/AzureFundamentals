using AzureServicesViews.Models;
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
        public async Task<IActionResult> AddFile(string containerName, Blob blob, IFormFile formFile)
        {
            if (formFile == null || formFile.Length < 1)
                return View();

            var fileName = Path.GetFileNameWithoutExtension(formFile.FileName) + "_" + Guid.NewGuid() + Path.GetExtension(formFile.FileName);
            var result = await _blobService.UploadBlob(fileName, formFile, containerName, blob);
            if (result)
                return RedirectToAction("Index", "Container");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewFile(string name, string containerName)
        {
            return Redirect(await _blobService.GetBlob(name, containerName));
        }
        [HttpGet]
        public async Task<IActionResult> DeleteFile(string name, string containerName)
        {
            var result = await _blobService.DeleteBlob(name, containerName);
            if (result) return RedirectToAction("Index", "Home");
            return View();
        }
    }
}
