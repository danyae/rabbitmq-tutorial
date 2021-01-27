using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using RabbitMq.Tutorial.Supplier.Database;

namespace RabbitMq.Tutorial.Supplier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController
    {
        private readonly SupplierDbContext _dbContext;
        
        public ProductsController(SupplierDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet("")]
        public IEnumerable<Product> Get()
        {
            return _dbContext.Products.ToList();
        }
    }
}
