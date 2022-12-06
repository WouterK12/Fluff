namespace Fluff
{
    public interface IBasicReceiver : IDisposable
    {
        void SetupQueue(IEnumerable<string> topics);
        void StartReceiving(Action<EventMessage> handler);
    }
}