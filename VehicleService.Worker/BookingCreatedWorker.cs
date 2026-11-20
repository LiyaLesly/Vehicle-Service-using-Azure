using System.Net;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using VehicleService.Messaging;

namespace VehicleService.Worker
{
    public class BookingCreatedWorker : BackgroundService
    {
        private const int MaxAttempts = 3;

        private readonly ServiceBusProcessor _processor;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingCreatedWorker> _logger;

        public BookingCreatedWorker(ServiceBusClient client, IConfiguration config,
            IEmailService emailService, ILogger<BookingCreatedWorker> logger)
        {
            _emailService = emailService;
            _logger = logger;
            _processor = client.CreateProcessor(config["ServiceBus:QueueName"],
                new ServiceBusProcessorOptions
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1
                });
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _processor.ProcessMessageAsync += HandleMessage;
            _processor.ProcessErrorAsync += HandleError;

            await _processor.StartProcessingAsync(ct);
            _logger.LogInformation("Listening on queue {Queue}", _processor.EntityPath);

            try { await Task.Delay(Timeout.Infinite, ct); }
            catch (OperationCanceledException) { }

            await _processor.StopProcessingAsync();
        }

        private async Task HandleMessage(ProcessMessageEventArgs args)
        {
            
            var message = args.Message;
            _logger.LogInformation("Received message {Id}", message.MessageId);

            BookingCreatedMessage? booking;
            try
            {
                booking = JsonSerializer.Deserialize<BookingCreatedMessage>(
     message.Body.ToString(),
     new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Invalid message {Id}", message.MessageId);
                await args.DeadLetterMessageAsync(message, "InvalidJson", ex.Message);
                return;
            }

            if (booking == null)
            {
                await args.DeadLetterMessageAsync(message, "EmptyMessage",
                    "Message body was empty");
                return;
            }

            try
            {
                var (subject, html) = BuildEmail(booking);
                await _emailService.SendAsync(booking.CustomerEmail, subject, html);

                _logger.LogInformation("Processed booking {Id} for {Email}",
                    booking.BookingId, booking.CustomerEmail);

                await args.CompleteMessageAsync(message);
            }
            catch (Exception ex)
            {
                if (message.DeliveryCount >= MaxAttempts)
                {
                    _logger.LogError(ex, "Giving up on booking {Id} after {Count} attempts",
                        booking.BookingId, message.DeliveryCount);
                    await args.DeadLetterMessageAsync(message, "ProcessingFailed", ex.Message);
                }
                else
                {
                    _logger.LogWarning(ex, "Attempt {Count} failed for booking {Id}. Retrying.",
                        message.DeliveryCount, booking.BookingId);
                    //await Task.Delay(TimeSpan.FromSeconds(5));
                    await args.AbandonMessageAsync(message);
                }
            }
        }

        private static (string Subject, string Html) BuildEmail(BookingCreatedMessage b)
        {
            string Enc(string s) => WebUtility.HtmlEncode(s);

            var subject = $"Service booking #{b.BookingId} received";
            var html =
                "<html><body>" +
                $"<h2>Hi {Enc(b.CustomerName)},</h2>" +
                "<p>We have received your vehicle service booking.</p>" +
                "<ul>" +
                $"<li><b>Booking ID:</b> {b.BookingId}</li>" +
                $"<li><b>Vehicle:</b> {Enc(b.VehicleDescription)}</li>" +
                $"<li><b>Date:</b> {b.BookingDate:dd-MM-yyyy hh:mm tt}</li>" +
                $"<li><b>Problem:</b> {Enc(b.ProblemDescription)}</li>" +
                "</ul>" +
                "<p>Thank you for choosing Vehicle Service.</p>" +
                "</body></html>";

            return (subject, html);
        }

        private Task HandleError(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "Service Bus error");
            return Task.CompletedTask;
        }
    }
}