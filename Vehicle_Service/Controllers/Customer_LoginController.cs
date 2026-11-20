
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Json;
using System.Security.Claims;
using Vehicle_Service.Models;

namespace Vehicle_Service.Controllers
{
    public class Customer_LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public Customer_LoginController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login_Pgload()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login_clk(Login obj)
        {
            if (!ModelState.IsValid)
                return View("Login_Pgload", obj);

            string baseUrl =
                _configuration["ApiSettings:BaseUrl"]!;

            var client = _httpClientFactory.CreateClient();

            var request = new ApiLoginRequest
            {
                Username = obj.Username,
                Password = obj.Password
            };

            try
            {
                var response = await client.PostAsJsonAsync(
                    $"{baseUrl.TrimEnd('/')}/api/Login",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    obj.msg = "Invalid username or password.";
                    return View("Login_Pgload", obj);
                }

                var result =
                    await response.Content.ReadFromJsonAsync<ApiLoginResponse>();

                if (result == null ||
                    string.IsNullOrWhiteSpace(result.Token))
                {
                    obj.msg = "Unable to complete login.";
                    return View("Login_Pgload", obj);
                }

                if (result.Role != "Customer" &&
                    result.Role != "ServiceManager")
                {
                    obj.msg = "Invalid user role.";
                    return View("Login_Pgload", obj);
                }

                // Store the JWT and user details in Session
                HttpContext.Session.SetString("jwt", result.Token);
                HttpContext.Session.SetString(
                    "uid", result.UserId.ToString());
                HttpContext.Session.SetString("role", result.Role);

                // Create the MVC authentication cookie
                var claims = new List<Claim>
                {
                    new Claim(
                        ClaimTypes.NameIdentifier,
                        result.UserId.ToString()),
                    new Claim(ClaimTypes.Role, result.Role)
                };

                var identity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal);

                // Redirect according to the user's role
                if (result.Role == "Customer")
                {
                    return RedirectToAction(
                        "Customer_Pgload",
                        "Customer_Home",
                        new { id = result.UserId });
                }

                return RedirectToAction(
                    "Manager_Pgload",
                    "Manager_Home",
                    new { id = result.UserId });
            }
            catch (HttpRequestException)
            {
                obj.msg = "Unable to connect to the API. Please try again.";
                return View("Login_Pgload", obj);
            }
        }
    }
}
