using Microsoft.AspNetCore.Mvc;
using Vehicle_Service.Models;

namespace Vehicle_Service.Controllers
{
    public class VehicleController : Controller
    {
        VehicleDB db = new VehicleDB();
        public IActionResult Vehicle_Pgload(int customerId)
        {
            var getvehicles = db.GetVehiclesDB(customerId);

            ViewBag.CustomerId = customerId;

            return View(getvehicles);
        }

        // Open Add Vehicle page
        //[HttpGet]
        public IActionResult Vehicle_Add(int customerId)
        {
            ViewBag.CustomerId = customerId;

            return View();
        }
        // Insert Vehicle

        [HttpPost]
        public IActionResult Vehicle_Add(Vehicle clsobj)
        {
            if (ModelState.IsValid)
            {
                db.Vehicle_InsertDB(clsobj);
                return RedirectToAction("Vehicle_Pgload",new { customerId = clsobj.CustomerId });
            }
            else
            {
                ViewBag.CustomerId = clsobj.CustomerId;
                ViewBag.Message = "Please enter all vehicle details";
                return View(clsobj);
            }
        }
    }
}
