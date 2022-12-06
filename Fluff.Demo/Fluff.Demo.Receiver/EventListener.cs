using Fluff.Demo.Models;
using Microsoft.Extensions.Logging;

namespace Fluff.Demo.Receiver;

[EventListener]
internal class EventListener
{
    private readonly ILogger<EventListener> _logger;

    public EventListener(ILogger<EventListener> logger)
    {
        _logger = logger;
    }

    [Handler(topic: "Fluff.Demo.MessageEvent")]
    public void Handle(MessageEvent evt)
    {
        _logger.LogInformation("[{0}] [{1}] {2}", evt.Timestamp, evt.CorrelationId, evt.Message);
    }
}
