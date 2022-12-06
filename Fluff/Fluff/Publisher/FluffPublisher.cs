using Fluff.Events;
using System.Text.Json;

namespace Fluff
{
    internal class FluffPublisher : IFluffPublisher
    {
        private readonly IBasicSender _sender;

        public FluffPublisher(IFluffContext context) : this(new BasicSender(context))
        {
        }

        internal FluffPublisher(IBasicSender sender)
        {
            _sender = sender;
        }

        public void Publish<T>(string topic, T evt) where T : DomainEvent
        {
            string body = JsonSerializer.Serialize(evt);
            EventMessage message = new(topic, body);

            _sender.Send(message);
        }
    }
}
