using RabbitMQ.Client;

namespace Fluff
{
    public class FluffOptions : IFluffOptions
    {
        public string HostName { get; init; } = FluffOptionsDefaults.HostName;
        public int Port { get; init; } = FluffOptionsDefaults.Port;

        public string UserName { get; init; } = FluffOptionsDefaults.UserName;
        public string Password { get; init; } = FluffOptionsDefaults.Password;

        public string ExchangeName { get; init; } = FluffOptionsDefaults.ExchangeName;
        public string ExchangeType { get; init; } = FluffOptionsDefaults.ExchangeType;
        public bool ExchangeIsDurable { get; init; } = FluffOptionsDefaults.ExchangeIsDurable;

        public string QueueName { get; init; } = FluffOptionsDefaults.QueueName;

        public IConnectionFactory CreateFactory()
        {
            return new ConnectionFactory
            {
                HostName = HostName,
                Port = Port,
                UserName = UserName,
                Password = Password
            };
        }
    }
}
