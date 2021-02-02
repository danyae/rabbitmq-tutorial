using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
        private const string QueueName = "rabbitmq.tutorial.orders";
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IModel _channel;

        public OrderConsumer(IServiceProvider services)
        {
            _serviceProvider = services;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() {HostName = "localhost", Port = 5672};
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
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
                _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(queue: QueueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
