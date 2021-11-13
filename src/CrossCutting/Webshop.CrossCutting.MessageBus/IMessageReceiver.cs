using Webshop.CrossCutting.MessageBus.Models;

namespace Webshop.CrossCutting.MessageBus
{
    public interface IMessageReceiver
    {
        void Subscribe<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEventHandler : IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent;
    }
}
