using System;
using System.IO;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Webshop.CrossCutting.MessageBus.RabbitMQ
{
    internal class DefaultRabbitMqConnection : IRabbitMqConnection
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly ILogger<DefaultRabbitMqConnection> logger;
        private readonly int retryCount;
        private IConnection connection;
        private bool disposed;

        private readonly object lockObject = new();

        public DefaultRabbitMqConnection(
            IOptions<RabbitMqConfiguration> settings,
            ILogger<DefaultRabbitMqConnection> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (settings?.Value == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            connectionFactory = new ConnectionFactory
            {
                HostName = settings.Value.HostName,
                DispatchConsumersAsync = true,
                UserName = settings.Value.UserName,
                Password = settings.Value.Password
            };

            retryCount = 5;
        }

        public bool IsConnected => connection != null && connection.IsOpen && !disposed;

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return connection.CreateModel();
        }

        public void Dispose()
        {
            if (disposed) return;

            disposed = true;

            try
            {
                connection.Dispose();
            }
            catch (IOException ex)
            {
                logger.LogCritical(ex.ToString());
            }
        }

        public bool TryConnect()
        {
            logger.LogInformation("RabbitMQ Client is trying to connect");

            lock (lockObject)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                    {
                        logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", $"{time.TotalSeconds:n1}", ex.Message);
                    }
                );

                policy.Execute(() =>
                {
                    connection = connectionFactory
                          .CreateConnection();
                });

                if (IsConnected)
                {
                    connection.ConnectionShutdown += OnConnectionShutdown;
                    connection.CallbackException += OnCallbackException;
                    connection.ConnectionBlocked += OnConnectionBlocked;

                    logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", connection.Endpoint.HostName);

                    return true;
                }

                logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (disposed) return;

            logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

            TryConnect();
        }

        void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (disposed) return;

            logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

            TryConnect();
        }

        void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (disposed) return;

            logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

            TryConnect();
        }
    }
}
