using RabbitMQ.Client;
using System.Text;

namespace Fluff
{
    internal class BasicSender : IBasicSender
    {
        private readonly IFluffContext _context;

        public BasicSender(IFluffContext context)
        {
            _context = context;
        }

        public void Send(EventMessage message)
        {
            using IModel channel = _context.CreateChannel();

            channel.BasicPublish(_context.ExchangeName,
                                  message.Topic,
                                  body: Encoding.Unicode.GetBytes(message.Body));
        }
    }
}
