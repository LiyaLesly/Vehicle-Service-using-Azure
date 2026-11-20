using Microsoft.Data.SqlClient;
using System.Data;

namespace VehicleServiceAPI.Data
{
    public class LoginDB
    {
        private readonly IConfiguration _configuration;

        public LoginDB(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (bool IsValid, int RegId, string Role)
            ValidateUser(string username, string password)
        {
            string connectionString =
                _configuration.GetConnectionString(
                    "DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is missing.");

            using SqlConnection con =
                new SqlConnection(connectionString);

            // 1. Validate username and password
            using SqlCommand cmd1 =
                new SqlCommand("sp_loginCountId", con);

            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.Parameters.AddWithValue("@una", username);
            cmd1.Parameters.AddWithValue("@pw", password);

            con.Open();

            int count = Convert.ToInt32(cmd1.ExecuteScalar());

            if (count == 0)
                return (false, 0, "");

            // 2. Get RegId
            int regId;

            using (SqlCommand cmd2 =
                new SqlCommand("sp_login", con))
            {
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.AddWithValue("@una", username);
                cmd2.Parameters.AddWithValue("@pw", password);

                object? value = cmd2.ExecuteScalar();

                regId = value == null || value == DBNull.Value
                    ? 0
                    : Convert.ToInt32(value);
            }

            // 3. Get Role
            string role;

            using (SqlCommand cmd3 =
                new SqlCommand("sp_roletype", con))
            {
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.Parameters.AddWithValue("@una", username);
                cmd3.Parameters.AddWithValue("@pw", password);

                object? value = cmd3.ExecuteScalar();

                role = value == null || value == DBNull.Value
                    ? ""
                    : value.ToString()!;
            }

            return (true, regId, role);
        }
    }
}