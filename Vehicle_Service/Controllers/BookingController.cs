using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Vehicle_Service.Models;
using VehicleService.Messaging;

namespace Vehicle_Service.Controllers
{
    public class BookingController : Controller
    {
        BookingDB dbob = new BookingDB();
        CustomerDB customerDb = new CustomerDB();

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IHttpClientFactory httpClientFactory,
            IConfiguration configuration, ILogger<BookingController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public IActionResult MyBookings(int customerId)
        {
            ViewBag.CustomerId = customerId;
            List<Booking> bookings = dbob.Booking_DetailsDB(customerId);
            return View(bookings);
        }

        [HttpGet]
        public IActionResult Booking_Pgload(int customerId)
        {
            ViewBag.CustomerId = customerId;
            ViewBag.VehicleDetails = dbob.GetVehiclesDB(customerId);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Booking_Add(Booking obj)
        {
            if (ModelState.IsValid)
            {
                obj.BookingDate = DateTime.Now;

                int bookingId = dbob.Booking_InsertDB(obj);

                if (bookingId > 0)
                {
                    await PublishBookingCreatedAsync(obj, bookingId);

                    TempData["Message"] = "Booking Added Successfully";

                    return RedirectToAction(
                        "BookingService_Pgload",
                        "BookingService",
                        new { bookingId = bookingId }
                    );
                }

                obj.msg = "Booking not added.";
            }

            ViewBag.CustomerId = obj.CustomerId;
            ViewBag.VehicleDetails = dbob.GetVehiclesDB(obj.CustomerId);

            return View("Booking_Pgload", obj);
        }

        private async Task PublishBookingCreatedAsync(Booking obj, int bookingId)
        {
            try
            {
                string email = customerDb.GetCustomerEmailDB(obj.CustomerId);
                if (string.IsNullOrWhiteSpace(email)) return;

                string? jwt = HttpContext.Session.GetString("jwt");
                if (string.IsNullOrWhiteSpace(jwt)) return;

                var vehicle = dbob.GetVehiclesDB(obj.CustomerId)
                    .FirstOrDefault(v => v.VehicleId == obj.VehicleId);

                var message = new BookingCreatedMessage
                {
                    BookingId = bookingId,
                    CustomerId = obj.CustomerId,
                    CustomerName = customerDb.GetCustomerNameDB(obj.CustomerId),
                    CustomerEmail = email,
                    VehicleDescription = vehicle == null
                        ? "Your vehicle"
                        : $"{vehicle.Brand} {vehicle.Model} ({vehicle.VehicleNumber})",
                    BookingDate = obj.BookingDate,
                    ProblemDescription = obj.ProblemDescription ?? ""
                };

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwt);

                string baseUrl = _configuration["ApiSettings:BaseUrl"]!;
                var response = await client.PostAsJsonAsync(
                    $"{baseUrl.TrimEnd('/')}/api/BookingNotification", message);

                if (!response.IsSuccessStatusCode)
                    _logger.LogWarning("Notification API returned {Code} for booking {Id}",
                        (int)response.StatusCode, bookingId);
            }
            catch (Exception ex)
            {
                // The booking is already saved, so a messaging problem must not break the page
                _logger.LogError(ex, "Could not publish message for booking {Id}", bookingId);
            }
        }
    }
}