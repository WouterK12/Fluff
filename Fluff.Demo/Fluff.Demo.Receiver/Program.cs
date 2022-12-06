using Fluff;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostcontext, services) =>
{
    services.AddFluff(new FluffOptions()
    {
        ExchangeName = "Fluff.DemoExchange",
        QueueName = "Fluff.DemoReceiver"
    });
});

builder.ConfigureLogging(builder =>
{
    builder.SetMinimumLevel(LogLevel.Debug);
});

var app = builder.Build();

app.Run();
