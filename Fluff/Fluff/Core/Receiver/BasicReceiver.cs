using Fluff.Exceptions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Fluff
{
    internal class BasicReceiver : IBasicReceiver
    {
        private readonly IFluffContext _context;
        private readonly ILogger<BasicReceiver> _logger;

        private IModel? _channel;
        private string? _consumerTag;

        public BasicReceiver(IFluffContext context, ILogger<BasicReceiver> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void SetupQueue(IEnumerable<string> topics)
        {
            if (_channel != null)
                throw new FluffException(FluffExceptionMessages.QueueAlreadySetup);

            if (topics == null || !topics.Any())
                _logger.LogWarning("Cannot create QueueBindings with no topics. Fluff can only be used as a sender.");

            _channel = _context.CreateChannel();

            _channel.QueueDeclare(_context.QueueName,
                                  durable: true,
                                  exclusive: false,
                                  autoDelete: false);

            foreach (string topic in topics)
            {
                _channel.QueueBind(queue: _context.QueueName,
                                   exchange: _context.ExchangeName,
                                   routingKey: topic);
            }
        }

        public void StartReceiving(Action<EventMessage> handler)
        {
            if (_channel == null)
                throw new FluffException(FluffExceptionMessages.NoQueue);

            if (!string.IsNullOrEmpty(_consumerTag))
                throw new FluffException(FluffExceptionMessages.AlreadyReceiving);

            EventingBasicConsumer consumer = new(_channel);

            consumer.Received += (_, e) =>
            {
                try
                {
                    EventMessage message = GetEventMessage(e);
                    handler?.Invoke(message);

                    _channel.BasicAck(e.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    _channel.BasicNack(e.DeliveryTag, multiple: false, requeue: true);
                }
            };

            _consumerTag = _channel.BasicConsume(_context.QueueName,
                                                 autoAck: false,
                                                 consumer);
        }

        private EventMessage GetEventMessage(BasicDeliverEventArgs e)
        {
            string body = Encoding.Unicode.GetString(e.Body.ToArray());

            return new EventMessage(e.RoutingKey, body);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _channel?.BasicCancel(_consumerTag);
            _channel?.Dispose();

            _context?.Dispose();
        }
    }
}
