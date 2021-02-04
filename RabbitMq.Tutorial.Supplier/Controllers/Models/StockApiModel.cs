using System;

namespace RabbitMq.Tutorial.Supplier.Controllers.Models
{
    public class StockApiModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public int Quantity { get; set; }

        public StockApiModel(Guid id, string name, int quantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
        }
    }
}
