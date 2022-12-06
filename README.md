# Fluff

A RabbitMQ Wrapper for .NET

## Usage

- Download the `.nupkg` file
- Install it in your project (using a custom NuGet feed)
- Run a local instance of RabbitMQ

## Examples

### Sender

Program.cs

```cs
services.AddFluff(new FluffOptions()
{
    ExchangeName = "Fluff.DemoExchange",
    QueueName = "Fluff.DemoSender"
});

IFluffPublisher publisher = services.BuildServiceProvider().GetRequiredService<IFluffPublisher>();

// inherits Fluff.Events.DomainEvent
MessageEvent evt = new("Hello!");

publisher.Publish("Fluff.Demo.MessageEvent", evt);
```

### Receiver

Program.cs

```cs
services.AddFluff(new FluffOptions()
{
    ExchangeName = "Fluff.DemoExchange",
    QueueName = "Fluff.DemoReceiver"
});
```

EventListener.cs

```cs
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
```

### Options with their defaults

```cs
services.AddFluff(new FluffOptions()
{
    HostName = "localhost",
    Port = 5672,
    UserName = "guest",
    Password = "guest",
    ExchangeName = "Fluff.DefaultExchange",
    ExchangeType = "Topic",
    ExchangeIsDurable = true,
    QueueName = "Fluff.DefaultQueue"
});
```

### Use environment variables

Add Fluff without `FluffOptions` to use the environment variables listed below.

```cs
services.AddFluff();

// Environment variables used:
// Fluff_HostName
// Fluff_Port
// Fluff_UserName
// Fluff_Password
// Fluff_ExchangeName
// Fluff_ExchangeType
// Fluff_ExchangeIsDurable
// Fluff_QueueName
```

More examples of how to use Fluff can be found in the [Demo project](./Fluff.Demo/).
