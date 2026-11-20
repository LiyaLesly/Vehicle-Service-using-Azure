
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Vehicle_Service.Models;

namespace Vehicle_Service.Controllers
{
    [Authorize(Roles = "Customer")]
    public class Customer_HomeController : Controller
    {
        CustomerDB db = new CustomerDB();

        public IActionResult Customer_Pgload(int id)
        {
            // Get the logged-in user's ID from the authentication cookie
            var loggedInUserId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (loggedInUserId == null ||
                !int.TryParse(loggedInUserId, out int customerId))
            {
                return Challenge();
            }

            // Prevent a customer from changing the URL to another customer's ID
            if (id != customerId)
            {
                return Forbid();
            }

            ViewBag.CustomerId = customerId;
            ViewBag.CustomerName = db.GetCustomerNameDB(customerId);

            return View();
        }
    }
}
