using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMq.Tutorial.Supplier.Messages;
using RabbitMq.Tutorial.Supplier.Options;

namespace RabbitMq.Tutorial.Supplier.Producers
{
    public class StockUpdateProducer
    {
        private const string ExchangeName = "rabbitmq.tutorial.stock-update";
        private readonly RabbitmqOptions _options;
        private readonly ILogger _logger;

        public StockUpdateProducer(ILogger<StockUpdateProducer> logger, IOptions<RabbitmqOptions> rabbitMqOptions)
        {
            _options = rabbitMqOptions.Value;
            _logger = logger;
        }

        public void Publish(StockUpdateMessage message)
        {
            if (message == null)
            {
                throw new InvalidOperationException($"Got empty message in {nameof(StockUpdateProducer)}");
            }
            
            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };
            
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);

                var body = JsonSerializer.SerializeToUtf8Bytes(message);
                channel.BasicPublish(exchange: ExchangeName,
                    routingKey: "",
                    basicProperties: null,
                    body: body);
                _logger.Log(LogLevel.Information, $"Sent {nameof(StockUpdateMessage)}: {JsonSerializer.Serialize(message)}");
            }
        }
    }
}
