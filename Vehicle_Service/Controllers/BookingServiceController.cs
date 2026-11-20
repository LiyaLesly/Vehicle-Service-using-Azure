using Microsoft.AspNetCore.Mvc;
using Vehicle_Service.Models;

namespace Vehicle_Service.Controllers
{
    public class BookingServiceController : Controller
    {
        BookingDB dbob = new BookingDB();
        // BOOKING SERVICE - Open form
        [HttpGet]
        public IActionResult BookingService_Pgload(int bookingId)
        {
            ViewBag.BookingId = bookingId;

            //ViewBag.CustomerId = customerId;

            ViewBag.Services = dbob.Service_DetailsDB();

            return View();
        }

        // BOOKING SERVICE - Save selected service
        [HttpPost]
        public IActionResult BookingService_Add(BookingService obj)
        {
            if (ModelState.IsValid)
            {
                string result = dbob.BookingService_InsertDB(obj);

                if (result == "Successfully Inserted")
                {
                    TempData["Message"] = "Service Added Successfully";

                    return RedirectToAction("BookingService_Pgload", "BookingService", new { bookingId = obj.BookingId }
                    );
                }

                ViewBag.Message = result;
            }

            ViewBag.BookingId = obj.BookingId;
            ViewBag.Services = dbob.Service_DetailsDB();

            return View("BookingService_Pgload", obj);
        }
    }

}
