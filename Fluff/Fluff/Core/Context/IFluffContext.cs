using RabbitMQ.Client;

namespace Fluff
{
    internal interface IFluffContext : IDisposable
    {
        IConnection Connection { get; }
        string ExchangeName { get; }
        string QueueName { get; }

        IModel CreateChannel();
    }
}