using Vehicle_Service.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Vehicle_Service.Controllers
{
    public class BillController : Controller
    {
        BillDB dbob = new BillDB();
        // Generate Bill
        public IActionResult GenerateBill(Bill clsob)
        {
            string result = dbob.Bill_InsertDB(clsob);

            TempData["Message"] = result;

            return RedirectToAction("Bill_Pgload", "Manager_Home");
        }
        public IActionResult Customer_Bill(int customerId)
        {
            var bills = dbob.Customer_Bill_DetailsDB(customerId);

            return View(bills);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult PayBill(int billId)
        {
            // Get the logged-in customer's ID
            string customerIdValue =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(customerIdValue, out int customerId))
            {
                return RedirectToAction("Login_Pgload", "Customer_Login");
            }

            string result = dbob.Bill_PayDB(billId, customerId);

            TempData["Message"] = result;

            return RedirectToAction("Customer_Bill",
                new { customerId = customerId });
        }

    }
}
