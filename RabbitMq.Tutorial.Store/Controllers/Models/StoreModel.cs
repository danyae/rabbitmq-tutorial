using System.Collections.Generic;
using RabbitMq.Tutorial.Store.Clients.Models;

namespace RabbitMq.Tutorial.Store.Controllers.Models
{
    public class StoreModel
    {
        public IEnumerable<Product> Products { get; private set; }
        public IEnumerable<Stock> Stocks { get; private set; }

        public StoreModel(IEnumerable<Product> products, IEnumerable<Stock> stocks)
        {
            Products = products;
            Stocks = stocks;
        }
    }
}
