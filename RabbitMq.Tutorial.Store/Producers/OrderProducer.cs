using System;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMq.Tutorial.Store.Messages;
using RabbitMq.Tutorial.Store.Options;

namespace RabbitMq.Tutorial.Store.Producers
{
    public class OrderProducer
    {
        private const string ExchangeName = "rabbitmq.tutorial.orders";
        private readonly RabbitmqOptions _options;
        private readonly ILogger _logger;

        public OrderProducer(ILogger<OrderProducer> logger, IOptions<RabbitmqOptions> rabbitMqOptions)
        {
            _options = rabbitMqOptions.Value;
            _logger = logger;
        }

        public void Publish(OrderMessage message)
        {
            if (message == null)
            {
                throw new InvalidOperationException($"Got empty message in {nameof(OrderProducer)}");
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
                var body = JsonSerializer.SerializeToUtf8Bytes(message);
                channel.BasicPublish(exchange: "",
                    routingKey: ExchangeName,
                    basicProperties: null,
                    body: body);
                _logger.Log(LogLevel.Information, $"Sent {nameof(OrderMessage)}: {JsonSerializer.Serialize(message)}");
            }
        }
    }
}
