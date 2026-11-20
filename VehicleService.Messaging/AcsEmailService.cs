using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VehicleService.Messaging
{
    public class AcsEmailService : IEmailService
    {
        private readonly EmailClient _client;
        private readonly string _sender;
        private readonly ILogger<AcsEmailService> _logger;

        public AcsEmailService(EmailClient client, IConfiguration config,
            ILogger<AcsEmailService> logger)
        {
            _client = client;
            _sender = config["Acs:Sender"]!;
            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            EmailSendOperation op = await _client.SendAsync(
                WaitUntil.Started, _sender, toEmail, subject, htmlBody);

            _logger.LogInformation("Email submitted to {Email}. Operation {Id}", toEmail, op.Id);
        }
    }
}