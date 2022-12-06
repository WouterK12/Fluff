using Fluff.Events;

namespace Fluff
{
    public interface IFluffPublisher
    {
        void Publish<T>(string topic, T evt) where T : DomainEvent;
    }
}