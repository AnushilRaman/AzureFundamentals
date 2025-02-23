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

        public HomeController(ILogger<HomeController> logger, IContainerServices containerServices)
        {
            _logger = logger;
            _containerServices = containerServices;
        }

        public async Task<IActionResult> Index()
        {

            return View(await _containerServices.GetAllContainerAndBlobs());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
