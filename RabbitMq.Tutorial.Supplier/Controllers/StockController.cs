using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Tutorial.Supplier.Controllers.Models;
using RabbitMq.Tutorial.Supplier.Database;

namespace RabbitMq.Tutorial.Supplier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController
    {
        private readonly SupplierDbContext _dbContext;

        public StockController(SupplierDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("")]
        public IEnumerable<StockApiModel> Get()
        {
            return _dbContext.WarehouseStockBalance
                .Include(x => x.Product)
                .Select(x => new StockApiModel(x.ProductId, x.Product.Name, x.Quantity))
                .ToList();
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(Guid productId, int quantity)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null)
            {
                return new BadRequestResult();
            }

            var stockBalance =
                await _dbContext.WarehouseStockBalance
                    .FirstOrDefaultAsync(x => x.ProductId == productId) ??
                new WarehouseStockBalance {ProductId = productId};

            stockBalance.Quantity += quantity;
            if (stockBalance.Quantity < 0)
            {
                return new BadRequestResult();
            }

            if (stockBalance.Id == default)
            {
                await _dbContext.WarehouseStockBalance.AddAsync(stockBalance);
            }

            await _dbContext.SaveChangesAsync();
            return new OkResult();
        }
    }
}
