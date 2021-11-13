using System.Threading.Tasks;

namespace Webshop.CrossCutting.MessageBus.Models
{
    public interface IIntegrationEventHandler<in TEvent> : IIntegrationEventHandler
        where TEvent : IntegrationEvent
    {
        Task Handle(TEvent @event);
    }

    public interface IIntegrationEventHandler
    {
    }
}
