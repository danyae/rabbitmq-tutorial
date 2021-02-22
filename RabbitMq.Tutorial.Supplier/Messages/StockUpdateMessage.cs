using System;

namespace RabbitMq.Tutorial.Supplier.Messages
{
    public class StockUpdateMessage
    {
        public Guid ProductId { get; set; }
        
        public string ProductName { get; set; }
        
        public int Quantity { get; set; }

        public double Price { get; set; }
    }
}
