using RabbitMQ.Client;

namespace Fluff
{
    public interface IFluffOptions
    {
        string HostName { get; init; }
        int Port { get; init; }

        string UserName { get; init; }
        string Password { get; init; }

        string ExchangeName { get; init; }
        string ExchangeType { get; init; }
        bool ExchangeIsDurable { get; init; }

        string QueueName { get; init; }

        IConnectionFactory CreateFactory();
    }
}
