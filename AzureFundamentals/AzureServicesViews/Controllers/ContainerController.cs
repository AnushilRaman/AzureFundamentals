using AzureServicesViews.Models;
using AzureServicesViews.Services;
using Microsoft.AspNetCore.Mvc;


namespace AzureServicesViews.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IContainerServices _container;

        public ContainerController(IContainerServices container)
        {
            this._container = container;
        }

        public async Task<IActionResult> Index()
        {
            var allContainer = await _container.GetAllContainer();
            return View(allContainer);
        }
        public async Task<IActionResult> Delete(string containerName)
        {
            await _container.DeleteContainer(containerName);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Create()
        {
            return View(new Container());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Container container)
        {
            await _container.CreateContainer(container.Name.ToLower());
            return RedirectToAction(nameof(Index));
        }
    }
}
