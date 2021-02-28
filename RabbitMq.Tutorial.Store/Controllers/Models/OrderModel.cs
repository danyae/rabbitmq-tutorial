using System;
using System.Collections.Generic;

namespace RabbitMq.Tutorial.Store.Controllers.Models
{
    public class OrderModel
    {
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        public Guid Id { get; set; }
        
        public int Count { get; set; }
    }
}
