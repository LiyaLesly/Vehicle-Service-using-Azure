using Microsoft.Extensions.Logging;

namespace VehicleService.Messaging
{
    public class NoOpMessagePublisher : IMessagePublisher
    {
        private readonly ILogger<NoOpMessagePublisher> _logger;

        public NoOpMessagePublisher(ILogger<NoOpMessagePublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            _logger.LogInformation("Messaging disabled. Skipped {Type}", typeof(T).Name);
            return Task.CompletedTask;
        }
    }
}