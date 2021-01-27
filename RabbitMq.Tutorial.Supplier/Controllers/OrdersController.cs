using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RabbitMq.Tutorial.Supplier.Database;

namespace RabbitMq.Tutorial.Supplier.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController
    {
        private readonly SupplierDbContext _dbContext;
        
        public OrdersController(SupplierDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet("")]
        public IEnumerable<Order> Get()
        {
            return _dbContext.Order
                .Include(x => x.OrderLines)
                .ToList();
        }
    }
}
