using RabbitMQ.Client;

namespace Fluff
{
    internal class FluffContext : IFluffContext
    {
        private readonly IFluffOptions _options;
        private IConnection? _connection;

        public IConnection Connection => GetConnection();

        public string ExchangeName => _options.ExchangeName;
        public string QueueName => _options.QueueName;

        private static readonly object _connectionLock = new();

        public FluffContext(IFluffOptions options)
        {
            _options = options;
        }

        private IConnection GetConnection()
        {
            return _connection ??= CreateConnection();
        }

        private IConnection CreateConnection()
        {
            lock (_connectionLock)
            {
                if (_connection != null)
                    return _connection;

                IConnectionFactory factory = _options.CreateFactory();
                IConnection connection = factory.CreateConnection();

                using IModel channel = connection.CreateModel();
                channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType, _options.ExchangeIsDurable);

                return connection;
            }
        }

        public IModel CreateChannel()
        {
            return Connection.CreateModel();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _connection?.Dispose();
        }
    }
}
