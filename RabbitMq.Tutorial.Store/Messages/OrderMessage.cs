using System;

namespace RabbitMq.Tutorial.Store.Messages
{
    public class OrderMessageLine
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderMessage
    {
        public Guid StoreId { get; set; }

        public OrderMessageLine[] OrderLines { get; set; }
    }
}
