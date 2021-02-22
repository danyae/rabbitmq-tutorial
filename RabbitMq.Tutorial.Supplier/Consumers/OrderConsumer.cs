using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMq.Tutorial.Supplier.Database;
using RabbitMq.Tutorial.Supplier.Messages;
using RabbitMq.Tutorial.Supplier.Producers;

namespace RabbitMq.Tutorial.Supplier.Consumers
{
    /* {
          "StoreId":"531317C9-5457-40C5-8EF3-6358A38587DF",
          "OrderLines":
          [
              {
                "ProductId": "49A226E0-3991-4B09-BF11-AF3DFAE6D0D2",
                "Quantity": 1
              }
          ]
       }
    */
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
                try
                {
                    var body = eventArgs.Body.ToArray();
                    var message = JsonSerializer.Deserialize<OrderMessage>(body);
                
                    _logger.LogInformation($"Got message from {message.StoreId}");

                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<SupplierDbContext>();
                        var order = new Order()
                        {
                            OrderLines = message.OrderLines.Select(x => new OrderLine()
                                {ProductId = x.ProductId, Quantity = x.Quantity}).ToList(),
                            StoreId = message.StoreId
                        };
                        dbContext.Add(order);
                        
                        var orderProductIds = message.OrderLines.Select(x => x.ProductId).ToArray();
                        var stock = dbContext.WarehouseStockBalance
                            .Include(x => x.Product)
                            .Where(x => orderProductIds.Contains(x.ProductId))
                            .ToArray();
                        foreach (var orderLine in message.OrderLines)
                        {
                            var stockItem = stock.FirstOrDefault(x => x.Id == orderLine.ProductId);
                            if (stockItem != null)
                            {
                                stockItem.Quantity -= orderLine.Quantity;    
                            }
                        }

                        dbContext.SaveChanges();

                        var stockProducer = scope.ServiceProvider.GetRequiredService<StockUpdateProducer>();
                        foreach (var record in stock)
                        {
                            stockProducer.Publish(new StockUpdateMessage()
                            {
                                ProductId = record.ProductId, 
                                ProductName = record.Product.Name,
                                Price = record.Product.Price, 
                                Quantity = record.Quantity
                            });
                        }
                    }

                    _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Exception in ${nameof(OrderConsumer)}");
                    throw;
                }
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
                    _logger.LogError(e, $"Exception in {QueueName} consumer");
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
