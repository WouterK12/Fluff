using Fluff.Events;

namespace Fluff.Test.Mocks
{
    [EventListener]
    internal class ListenerMock
    {
        public const string Topic = "Fluff.Test1.*";

        [Handler(Topic)]
        public void Handle(DomainEvent evt)
        {
        }
    }
}
