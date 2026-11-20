using Microsoft.Extensions.Logging;

namespace VehicleService.Messaging
{
    public class LogEmailService : IEmailService
    {
        private readonly ILogger<LogEmailService> _logger;

        public LogEmailService(ILogger<LogEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            _logger.LogInformation("[LOG ONLY] Email to {Email}: {Subject}", toEmail, subject);
            return Task.CompletedTask;
        }
    }
}