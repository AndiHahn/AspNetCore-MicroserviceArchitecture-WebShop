namespace Webshop.CrossCutting.MessageBus.RabbitMQ
{
    public class RabbitMqConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string BrokerName { get; set; }
        public string QueueName { get; set; }
    }
}
