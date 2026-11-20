using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VehicleServiceAPI.Data;
using VehicleServiceAPI.Models;

namespace VehicleServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LoginDB _db;
        private readonly IConfiguration _configuration;

        public LoginController(
            LoginDB db,
            IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginRequest obj)
        {
            if (string.IsNullOrWhiteSpace(obj.Username) ||
                string.IsNullOrWhiteSpace(obj.Password))
            {
                return BadRequest(new
                {
                    message = "Username and password are required."
                });
            }

            var result = _db.ValidateUser(
                obj.Username, obj.Password);

            if (!result.IsValid)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password."
                });
            }

            if (result.Role != "Customer" &&
                result.Role != "ServiceManager")
            {
                return Forbid();
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    result.RegId.ToString()),

                new Claim(
                    ClaimTypes.Role,
                    result.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            int expiryMinutes = int.Parse(
                _configuration["Jwt:ExpiryMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    expiryMinutes),
                signingCredentials: credentials);

            string jwt = new JwtSecurityTokenHandler()
                .WriteToken(token);

            return Ok(new
            {
                token = jwt,
                userId = result.RegId,
                role = result.Role,
                expiresInMinutes = expiryMinutes
            });
        }
    }
}