using Microsoft.AspNetCore.Mvc;
using Vehicle_Service.Models;

namespace Vehicle_Service.Controllers
{
    public class Manager_HomeController : Controller
    {
        ServiceDB dbob = new ServiceDB();
        public IActionResult Manager_Pgload()
        {
            return View();
        }

        // Open Services
        public IActionResult Service_Pgload()
        {
            ViewBag.ServiceDetails = dbob.Service_DetailsDB();

            return View();
        }

        //Add Services
        [HttpPost]
        public IActionResult Service_Pgload(Service clsobj)
        {
            if (ModelState.IsValid)
            {
                string ins = dbob.Service_InsertDB(clsobj);

                TempData["Message"] = ins;

                return RedirectToAction("Service_Pgload");
            }

            else
            {
                TempData["Message"] = "Service is not Added";
               
                ViewBag.ServiceDetails = dbob.Service_DetailsDB();

                return View(clsobj);
            }
        }
        //Manage Bookings
        public IActionResult ManagerBooking_Pgload()
        {
            var bookings = dbob.Manager_Booking_ViewDB();

            return View(bookings);
        }
        //Manage Bills
        public IActionResult Bill_Pgload()
        {
            var billdetails = dbob.Manager_Bill_ViewDB();

            return View(billdetails);
        }
        
    }
}
