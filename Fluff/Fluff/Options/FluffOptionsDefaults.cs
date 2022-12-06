namespace Fluff
{
    public readonly struct FluffOptionsDefaults
    {
        public const string HostName = "localhost";
        public const int Port = 5672;

        public const string UserName = "guest";
        public const string Password = "guest";

        public const string ExchangeName = "Fluff.DefaultExchange";
        public const string ExchangeType = RabbitMQ.Client.ExchangeType.Topic;
        public const bool ExchangeIsDurable = true;

        public const string QueueName = "Fluff.DefaultQueue";
    }
}
