using System;
using RabbitMQ.Client;

namespace Webshop.CrossCutting.MessageBus.RabbitMQ
{
    public interface IRabbitMqConnection : IDisposable
    {
        bool IsConnected { get; }
        bool TryConnect();
        IModel CreateModel();
    }
}
