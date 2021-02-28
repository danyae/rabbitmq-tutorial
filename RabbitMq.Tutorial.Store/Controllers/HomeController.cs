using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMq.Tutorial.Store.Clients;
using RabbitMq.Tutorial.Store.Controllers.Models;
using RabbitMq.Tutorial.Store.Messages;
using RabbitMq.Tutorial.Store.Models;
using RabbitMq.Tutorial.Store.Options;
using RabbitMq.Tutorial.Store.Producers;

namespace RabbitMq.Tutorial.Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly StoreOptions _options;
        private readonly SupplierClient _supplierClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IOptions<StoreOptions> options,
            SupplierClient supplierClient,
            ILogger<HomeController> logger)
        {
            _options = options.Value;
            _supplierClient = supplierClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<StoreModel>> Index(CancellationToken ct)
        {
            ViewBag.StoreName = _options.Name;

            var products = (await _supplierClient.GetProducts(ct)).ToArray();
            var stocks = await _supplierClient.GetStocks(ct);
            foreach (var product in products)
            {
                product.Price *= _options.Markup;
            }

            return View(new StoreModel(products, stocks));
        }

        [HttpPost]
        public IActionResult Index(
            OrderModel model,
            [FromServices] OrderProducer producer)
        {
            _logger.LogInformation(JsonSerializer.Serialize(model));
            var orderMessage = new OrderMessage()
            {
                StoreId = _options.Id,
                OrderLines = model.OrderLines.Select(x => new OrderMessageLine {ProductId = x.Id, Quantity = x.Count})
                    .ToArray()
            };
            producer.Publish(orderMessage);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}
