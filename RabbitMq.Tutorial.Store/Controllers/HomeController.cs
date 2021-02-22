using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Tutorial.Store.Models;
using RabbitMq.Tutorial.Store.Options;

namespace RabbitMq.Tutorial.Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly StoreOptions _options;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<StoreOptions> options,
            ILogger<HomeController> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.StoreName = _options.Name;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
