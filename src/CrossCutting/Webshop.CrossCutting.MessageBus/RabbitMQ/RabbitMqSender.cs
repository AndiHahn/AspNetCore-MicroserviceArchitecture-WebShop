using System;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Webshop.CrossCutting.MessageBus.Models;

namespace Webshop.CrossCutting.MessageBus.RabbitMQ
{
    public class RabbitMqSender : IMessageSender
    {
        private readonly IRabbitMqConnection connection;
        private readonly RabbitMqConfiguration settings;
        private readonly ILogger<RabbitMqSender> logger;
        private readonly int retryCount;

        public RabbitMqSender(
            IRabbitMqConnection connection,
            IOptions<RabbitMqConfiguration> settings,
            ILogger<RabbitMqSender> logger,
            int retryCount = 5)
        {
            this.connection = connection ?? throw new System.ArgumentNullException(nameof(connection));
            this.settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.retryCount = retryCount;
        }

        public void Publish(IntegrationEvent @event)
        {
            if (!connection.IsConnected)
            {
                connection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                .Or<SocketException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {
                    logger.LogWarning(ex, "Could not publish event: {EventId} after {Timeout}s ({ExceptionMessage})", @event.Id, $"{time.TotalSeconds:n1}", ex.Message);
                });

            var eventName = @event.GetType().Name;

            logger.LogTrace("Creating RabbitMQ channel to publish event: {EventId} ({EventName})", @event.Id, eventName);

            using var channel = connection.CreateModel();
            logger.LogTrace("Declaring RabbitMQ exchange to publish event: {EventId}", @event.Id);

            channel.ExchangeDeclare(exchange: settings.BrokerName, type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = channel.CreateBasicProperties();
                properties.DeliveryMode = 2; // persistent

                logger.LogTrace("Publishing event to RabbitMQ: {EventId}", @event.Id);

                channel.BasicPublish(
                    exchange: settings.BrokerName,
                    routingKey: eventName,
                    mandatory: true,
                    basicProperties: properties,
                    body: body);
            });
        }
    }
}
