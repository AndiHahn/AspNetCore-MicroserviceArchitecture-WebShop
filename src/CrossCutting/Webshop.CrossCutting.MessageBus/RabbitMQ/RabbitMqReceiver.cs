using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Webshop.CrossCutting.MessageBus.Extensions;
using Webshop.CrossCutting.MessageBus.Models;

namespace Webshop.CrossCutting.MessageBus.RabbitMQ
{
    public class RabbitMqReceiver : IMessageReceiver
    {
        private readonly IRabbitMqConnection connection;
        private readonly ISubscriptionsManager subscriptionsManager;
        private readonly IServiceProvider serviceProvider;
        private readonly RabbitMqConfiguration settings;
        private readonly ILogger<RabbitMqReceiver> logger;

        private IModel consumerChannel;

        public RabbitMqReceiver(
            IRabbitMqConnection connection,
            ISubscriptionsManager subscriptionsManager,
            IOptions<RabbitMqConfiguration> settings,
            IServiceProvider serviceProvider,
            ILogger<RabbitMqReceiver> logger)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.subscriptionsManager = subscriptionsManager ?? throw new ArgumentNullException(nameof(subscriptionsManager));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

            consumerChannel = CreateConsumerChannel();
        }

        public void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = subscriptionsManager.GetEventKey<TEvent>();
            DoInternalSubscription(eventName);

            logger.LogInformation("Subscribing to event {EventName} with {EventHandler}", eventName, typeof(TEventHandler).GetGenericTypeName());

            subscriptionsManager.AddSubscription<TEvent, TEventHandler>();
            StartBasicConsume();
        }

        public void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = subscriptionsManager.GetEventKey<TEvent>();

            logger.LogInformation("Unsubscribing from event {EventName}", eventName);

            subscriptionsManager.RemoveSubscription<TEvent, TEventHandler>();
        }

        private void DoInternalSubscription(string eventName)
        {
            var containsKey = subscriptionsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                if (!connection.IsConnected)
                {
                    connection.TryConnect();
                }

                using var channel = connection.CreateModel();
                channel.QueueBind(queue: settings.QueueName,
                    exchange: settings.BrokerName,
                    routingKey: eventName);
            }
        }

        private void StartBasicConsume()
        {
            logger.LogTrace("Starting RabbitMQ basic consume");

            if (consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(
                    queue: settings.QueueName,
                    autoAck: false,
                    consumer: consumer);
            }
            else
            {
                logger.LogError("StartBasicConsume can't call on _consumerChannel == null");
            }
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var message = Encoding.UTF8.GetString(eventArgs.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "----- ERROR Processing message \"{Message}\"", message);
            }

            consumerChannel.BasicAck(eventArgs.DeliveryTag, multiple: false);
        }

        private IModel CreateConsumerChannel()
        {
            if (!connection.IsConnected)
            {
                connection.TryConnect();
            }

            logger.LogTrace("Creating RabbitMQ consumer channel");

            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: settings.BrokerName,
                                    type: "direct");

            channel.QueueDeclare(queue: settings.QueueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                logger.LogWarning(ea.Exception, "Recreating RabbitMQ consumer channel");

                consumerChannel.Dispose();
                consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            logger.LogTrace("Processing RabbitMQ event: {EventName}", eventName);

            if (subscriptionsManager.HasSubscriptionsForEvent(eventName))
            {
                using var scope = serviceProvider.CreateScope();
                var subscriptions = subscriptionsManager.GetHandlersForEvent(eventName);
                foreach (var subscription in subscriptions)
                {
                    var handler = scope.ServiceProvider.GetService(subscription);
                    if (handler == null) continue;
                    var eventType = subscriptionsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new[] { integrationEvent });
                }
            }
            else
            {
                logger.LogWarning("No subscription for RabbitMQ event: {EventName}", eventName);
            }
        }
    }
}
