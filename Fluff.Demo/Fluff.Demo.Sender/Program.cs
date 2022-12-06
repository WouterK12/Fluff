using Fluff;
using Fluff.Demo.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostcontext, services) =>
{
    services.AddFluff(new FluffOptions()
    {
        ExchangeName = "Fluff.DemoExchange",
        QueueName = "Fluff.DemoSender"
    });

    IFluffPublisher publisher = services.BuildServiceProvider().GetRequiredService<IFluffPublisher>();

    MessageEvent evt = new("Hello!");

    publisher.Publish("Fluff.Demo.MessageEvent", evt);
});

builder.ConfigureLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug);
});

var app = builder.Build();

app.Run();
