using System;
using System.Text.Json.Serialization;

namespace RabbitMq.Tutorial.Store.Clients.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public double Price { get; set; }
    }
}
