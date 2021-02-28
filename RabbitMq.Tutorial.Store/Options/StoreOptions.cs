using System;

namespace RabbitMq.Tutorial.Store.Options
{
    public class StoreOptions
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        /// <summary>
        /// the amount added to the cost price of goods (for example 1.2 means 20%)
        /// </summary>
        public double Markup { get; set; }
    }
}
