using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Tutorial.Store.Clients;
using RabbitMq.Tutorial.Store.Models;
using RabbitMq.Tutorial.Store.Options;

namespace RabbitMq.Tutorial.Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly StoreOptions _options;
        private readonly SupplierClient _supplierClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IOptions<StoreOptions> options,
            SupplierClient supplierClient,
            ILogger<HomeController> logger)
        {
            _options = options.Value;
            _supplierClient = supplierClient;
            _logger = logger;
        }

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            ViewBag.StoreName = _options.Name;

            var products = (await _supplierClient.GetProducts(ct)).ToArray();
            foreach (var product in products)
            {
                product.Price = product.Price * _options.Markup;
            }
            
            return View(products);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
