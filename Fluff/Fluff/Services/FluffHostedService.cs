using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fluff
{
    internal class FluffHostedService : IHostedService
    {
        private readonly IBasicReceiver _receiver;
        private readonly IFluffRouter _router;

        private readonly ILogger<FluffHostedService> _logger;

        public FluffHostedService(IBasicReceiver receiver, IFluffRouter router, ILogger<FluffHostedService> logger)
        {
            _receiver = receiver;
            _router = router;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _receiver.SetupQueue(_router.Topics);
            _receiver.StartReceiving(_router.Route);

            _logger.LogInformation("Started");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _receiver.Dispose();

            _logger.LogInformation("Disposed");

            return Task.CompletedTask;
        }
    }
}
