using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMq.Tutorial.Supplier.Database;
using RabbitMq.Tutorial.Supplier.Messages;

namespace RabbitMq.Tutorial.Supplier.BackgroundServices
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderConsumer(IServiceProvider services)
        {
            _serviceProvider = services;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "rabbitmq.tutorial.orders",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.Span;
                var message = JsonSerializer.Deserialize<OrderMessage>(body);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<SupplierDbContext>();
                    dbContext.Add(new Order()
                    {
                        OrderLines = message.OrderLines.Select(x => new OrderLine()
                            {ProductId = x.ProductId, Quantity = x.Quantity}).ToList(),
                        StoreId = message.StoreId
                    });

                    dbContext.SaveChanges();
                }
            };

            return Task.CompletedTask;
        }
    }
}
