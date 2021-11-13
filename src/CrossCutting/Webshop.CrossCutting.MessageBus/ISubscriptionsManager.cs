using System;
using System.Collections.Generic;
using Webshop.CrossCutting.MessageBus.Models;

namespace Webshop.CrossCutting.MessageBus
{
    public interface ISubscriptionsManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved;
        void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>;

        void RemoveSubscription<TEvent, TEventHandler>()
            where TEventHandler : IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent;
        bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent;
        bool HasSubscriptionsForEvent(string eventName);
        Type GetEventTypeByName(string eventName);
        void Clear();
        IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent;
        IEnumerable<Type> GetHandlersForEvent(string eventName);
        string GetEventKey<T>();
    }
}
