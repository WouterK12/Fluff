using Fluff.Events;

namespace Fluff.Demo.Models;

public class MessageEvent : DomainEvent
{
    public string Message { get; init; }

    public MessageEvent(string message) : base()
    {
        Message = message;
    }
}