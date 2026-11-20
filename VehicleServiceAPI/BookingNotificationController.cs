using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.Messaging;

namespace VehicleServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingNotificationController : ControllerBase
    {
        private readonly IMessagePublisher _publisher;
        private readonly ILogger<BookingNotificationController> _logger;

        public BookingNotificationController(IMessagePublisher publisher,
            ILogger<BookingNotificationController> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<IActionResult> Publish([FromBody] BookingCreatedMessage message,
            CancellationToken ct)
        {
            if (!int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    out int customerId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(message.CustomerEmail))
                return BadRequest(new { message = "Customer email is required." });

            message.CustomerId = customerId;

            await _publisher.PublishAsync(message, ct);

            _logger.LogInformation("Published booking {Id}", message.BookingId);
            return Accepted();
        }
    }
}