using AzureServicesViews.Models;
using AzureServicesViews.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureServicesViews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContainerServices _containerServices;
        private readonly IBlobService _blobService;

        public HomeController(ILogger<HomeController> logger, IContainerServices containerServices, IBlobService blobService)
        {
            _logger = logger;
            _containerServices = containerServices;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _containerServices.GetAllContainerAndBlobs());
        }

        public async Task<IActionResult> Images()
        {
            return View(await _blobService.GetAllBlobsWithUri("dotnetcorecontainer-image"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
