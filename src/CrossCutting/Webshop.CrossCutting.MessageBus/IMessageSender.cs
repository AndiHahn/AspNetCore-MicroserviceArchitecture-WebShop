using Webshop.CrossCutting.MessageBus.Models;

namespace Webshop.CrossCutting.MessageBus
{
    public interface IMessageSender
    {
        void Publish(IntegrationEvent @event);
    }
}
