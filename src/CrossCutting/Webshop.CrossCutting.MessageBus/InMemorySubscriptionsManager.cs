using System;
using System.Collections.Generic;
using System.Linq;
using Webshop.CrossCutting.MessageBus.Models;

namespace Webshop.CrossCutting.MessageBus
{
    internal class InMemorySubscriptionsManager : ISubscriptionsManager
    {
        private readonly Dictionary<string, List<Type>> handlers;
        private readonly List<Type> eventTypes;

        public event EventHandler<string> OnEventRemoved;

        public InMemorySubscriptionsManager()
        {
            handlers = new Dictionary<string, List<Type>>();
            eventTypes = new List<Type>();
        }

        public bool IsEmpty => !handlers.Keys.Any();
        public void Clear() => handlers.Clear();

        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : IntegrationEvent
            where TEventHandler : IIntegrationEventHandler<TEvent>
        {
            var eventName = GetEventKey<TEvent>();

            DoAddSubscription(typeof(TEventHandler), eventName);

            if (!eventTypes.Contains(typeof(TEvent)))
            {
                eventTypes.Add(typeof(TEvent));
            }
        }

        private void DoAddSubscription(Type handlerType, string eventName)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                handlers.Add(eventName, new List<Type>());
            }

            if (handlers[eventName].Any(s => s == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already registered for '{eventName}'", nameof(handlerType));
            }

            handlers[eventName].Add(handlerType);
        }

        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEventHandler : IIntegrationEventHandler<TEvent>
            where TEvent : IntegrationEvent
        {
            var handlerToRemove = FindSubscriptionToRemove<TEvent, TEventHandler>();
            var eventName = GetEventKey<TEvent>();
            DoRemoveHandler(eventName, handlerToRemove);
        }


        private void DoRemoveHandler(string eventName, Type subsToRemove)
        {
            if (subsToRemove != null)
            {
                handlers[eventName].Remove(subsToRemove);
                if (!handlers[eventName].Any())
                {
                    handlers.Remove(eventName);
                    var eventType = eventTypes.SingleOrDefault(e => e.Name == eventName);
                    if (eventType != null)
                    {
                        eventTypes.Remove(eventType);
                    }
                    RaiseOnEventRemoved(eventName);
                }
            }
        }

        public IEnumerable<Type> GetHandlersForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return GetHandlersForEvent(key);
        }

        public IEnumerable<Type> GetHandlersForEvent(string eventName) => handlers[eventName];

        private void RaiseOnEventRemoved(string eventName)
        {
            var handler = OnEventRemoved;
            handler?.Invoke(this, eventName);
        }

        private Type FindSubscriptionToRemove<T, TH>()
             where T : IntegrationEvent
             where TH : IIntegrationEventHandler<T>
        {
            var eventName = GetEventKey<T>();
            return DoFindSubscriptionToRemove(eventName, typeof(TH));
        }

        private Type DoFindSubscriptionToRemove(string eventName, Type handlerType)
        {
            if (!HasSubscriptionsForEvent(eventName))
            {
                return null;
            }

            return handlers[eventName].SingleOrDefault(s => s == handlerType);

        }

        public bool HasSubscriptionsForEvent<T>() where T : IntegrationEvent
        {
            var key = GetEventKey<T>();
            return HasSubscriptionsForEvent(key);
        }

        public bool HasSubscriptionsForEvent(string eventName) => handlers.ContainsKey(eventName);

        public Type GetEventTypeByName(string eventName) => eventTypes.SingleOrDefault(t => t.Name == eventName);

        public string GetEventKey<T>()
        {
            return typeof(T).Name;
        }
    }
}
