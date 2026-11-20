
namespace Vehicle_Service.Models
{
    public class ApiLoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class ApiLoginResponse
    {
        public string? Token { get; set; }
        public int UserId { get; set; }
        public string? Role { get; set; }
        public int ExpiresInMinutes { get; set; }
    }
}
