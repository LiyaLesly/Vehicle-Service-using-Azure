using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VehicleServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [Authorize]
        [HttpGet("protected")]
        public IActionResult Protected()
        {
            return Ok("Valid JWT token. Access granted.");
        }

        [Authorize(Roles = "ServiceManager")]
        [HttpGet("manager")]
        public IActionResult ManagerOnly()
        {
            return Ok("Service Manager access granted.");
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("customer")]
        public IActionResult CustomerOnly()
        {
            return Ok("Customer access granted.");
        }
    }
}