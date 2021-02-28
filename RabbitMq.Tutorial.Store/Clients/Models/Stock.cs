using System;

namespace RabbitMq.Tutorial.Store.Clients.Models
{
    public class Stock
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public int Quantity { get; set; }

        public Stock(Guid id, string name, int quantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
        }
    }
}
