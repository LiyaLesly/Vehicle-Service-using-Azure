using Microsoft.AspNetCore.Mvc;
using Vehicle_Service.Models;

namespace Vehicle_Service.Controllers
{
    public class CustomerInsertController : Controller
    {
        CustomerDB dbob = new CustomerDB();
        public IActionResult Index_pgload()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Insert_clk(Customer clsob)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int r_id = dbob.GetCustomerId();

                    string customer_ins = dbob.Customer_InsertDB(clsob, r_id);

                    if (customer_ins == "Successfully Inserted")
                    {
                        string users_ins = dbob.Users_InsertDB(clsob, r_id);

                        if (users_ins == "Successfully Inserted")
                        {
                            clsob.customer_msg ="Customer Registration Successfully Completed with Customer ID = " + r_id;
                        }
                        else
                        {
                            clsob.customer_msg = users_ins;
                        }
                    }
                    else
                    {
                        clsob.customer_msg = customer_ins;
                    }

                    return View("Index_pgload", clsob);
                }

                return View("Index_pgload", clsob);
            }
            catch (Exception ex)
            {
                clsob.customer_msg = ex.Message;
                return View("Index_pgload", clsob);
            }
        }
    }
}
