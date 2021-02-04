using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMq.Tutorial.Supplier.Database;
using RabbitMq.Tutorial.Supplier.Messages;

namespace RabbitMq.Tutorial.Supplier.BackgroundServices
{
    public class OrderConsumer : BackgroundService
    {
        private const string QueueName = "rabbitmq.tutorial.orders";
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;

        public OrderConsumer(IServiceProvider services, ILogger<OrderConsumer> logger)
        {
            _serviceProvider = services;
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            OpenConnection();
            
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

        private void OpenConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            // "if initial client connection to a RabbitMQ node fails, automatic connection recovery won't kick in."
            // but thats what we want in our case of a simple docker-compose file
            do
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                
                    _channel.QueueDeclare(queue: QueueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);
                    _channel.BasicQos(0, 1, false);
                    break;
                }
                catch (BrokerUnreachableException e)
                {
                    _logger.LogError(e, $"Exception in {nameof(OrderConsumer)}");
                    Thread.Sleep(5000);
                }
            } while (true);
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
