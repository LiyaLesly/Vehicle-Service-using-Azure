using System.Data;
using System.Data.SqlClient;

namespace Vehicle_Service.Models
{
    public class CustomerDB
    {
        SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true");
        public int GetCustomerId()
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_MaxIdLogin", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                int max_id = Convert.ToInt32(cmd.ExecuteScalar());

                int r_id = max_id + 1;

                con.Close();

                return r_id;
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                throw;
            }
        }

        public string GetCustomerNameDB(int customerId)
        {
            string customerName = "";

            using (SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true"))
            {
             SqlCommand cmd = new SqlCommand("select CustomerName from Customer where CustomerId = @CustomerId",con);

                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                con.Open();

                object? result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    customerName = result.ToString()?? "";
                }
            }

            return customerName;
        }

        //for ACS
        public string GetCustomerEmailDB(int customerId)
        {
            string email = "";

            using (SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true"))
            {
                SqlCommand cmd = new SqlCommand(
                    "select Email from Customer where CustomerId = @CustomerId", con);

                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                con.Open();
                object? result = cmd.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    email = result.ToString() ?? "";
                }
            }

            return email;
        }
        public string Customer_InsertDB(Customer clsobj, int r_id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_customer_insert", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@cid", r_id);
                cmd.Parameters.AddWithValue("@cna", clsobj.Cname);
                cmd.Parameters.AddWithValue("@addr", clsobj.Caddr);
                cmd.Parameters.AddWithValue("@phone", clsobj.Cphone);
                cmd.Parameters.AddWithValue("@email", clsobj.Email);
                cmd.Parameters.AddWithValue("@regdate", clsobj.Date);
                cmd.Parameters.AddWithValue("@status", "Active");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return "Successfully Inserted";
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                return "Customer Insert Error: " + ex.Message;
            }
        }
        public string Users_InsertDB(Customer clsobj, int r_id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_users_insert", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@regid", r_id);
                cmd.Parameters.AddWithValue("@una", clsobj.una);
                cmd.Parameters.AddWithValue("@pw", clsobj.pw);
                cmd.Parameters.AddWithValue("@role", "Customer");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return "Successfully Inserted";
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                return "User Insert Error: " + ex.Message;
            }
        }
        //Below code uses sp_loginCountId,sp_login,sp_roletype

        //public LoginResult LoginDB(Login obj)
        //{
        //    LoginResult result = new LoginResult();

        //    try
        //    {
        //        // 1. Check whether username and password are valid
        //        SqlCommand cmd1 = new SqlCommand("sp_loginCountId", con);
        //        cmd1.CommandType = CommandType.StoredProcedure;

        //        cmd1.Parameters.AddWithValue("@una", obj.Username);
        //        cmd1.Parameters.AddWithValue("@pw", obj.Password);

        //        con.Open();

        //        int count = Convert.ToInt32(cmd1.ExecuteScalar());

        //        con.Close();


        //        // 2. If login is valid
        //        if (count > 0)
        //        {
        //            // Get RegId
        //            SqlCommand cmd2 = new SqlCommand("sp_login", con);
        //            cmd2.CommandType = CommandType.StoredProcedure;

        //            cmd2.Parameters.AddWithValue("@una", obj.Username);
        //            cmd2.Parameters.AddWithValue("@pw", obj.Password);

        //            con.Open();

        //            object regId = cmd2.ExecuteScalar();

        //            if (regId != null && regId != DBNull.Value)
        //            {
        //                result.RegId = Convert.ToInt32(regId);
        //            }
        //            else
        //            {
        //                result.RegId = 0;
        //            }

        //            con.Close();


        //            // 3. Get Role
        //            SqlCommand cmd3 = new SqlCommand("sp_roletype", con);
        //            cmd3.CommandType = CommandType.StoredProcedure;

        //            cmd3.Parameters.AddWithValue("@una", obj.Username);
        //            cmd3.Parameters.AddWithValue("@pw", obj.Password);

        //            con.Open();

        //            object role = cmd3.ExecuteScalar();

        //            if (role != null && role != DBNull.Value)
        //            {
        //                result.Role = role.ToString();
        //            }

        //            con.Close();

        //            result.IsValid = true;
        //        }
        //        else
        //        {
        //            result.IsValid = false;
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        if (con.State == ConnectionState.Open)
        //        {
        //            con.Close();
        //        }

        //        throw new Exception("Login Error: " + ex.Message);
        //    }
        //}

        //Below code uses sp_loginCountId,sp_login,sp_roletype
        public LoginResult LoginDB(Login obj)
        {
            LoginResult result = new LoginResult();

            try
            {
                // 1. Check Username and Password
                SqlCommand cmd1 = new SqlCommand("sp_loginCountId", con);
                cmd1.CommandType = CommandType.StoredProcedure;

                cmd1.Parameters.AddWithValue("@una", obj.Username);
                cmd1.Parameters.AddWithValue("@pw", obj.Password);

                con.Open();
                int count = Convert.ToInt32(cmd1.ExecuteScalar());
                con.Close();

                // Invalid login
                if (count == 0)
                {
                    result.IsValid = false;
                    return result;
                }


                // 2. Get RegId
                SqlCommand cmd2 = new SqlCommand("sp_login", con);
                cmd2.CommandType = CommandType.StoredProcedure;

                cmd2.Parameters.AddWithValue("@una", obj.Username);
                cmd2.Parameters.AddWithValue("@pw", obj.Password);

                con.Open();

                object regId = cmd2.ExecuteScalar();

                if (regId != null && regId != DBNull.Value)
                {
                    result.RegId = Convert.ToInt32(regId);
                }
                else
                {
                    // Admin has RegId = NULL
                    result.RegId = 0;
                }

                con.Close();


                // 3. Get Role
                SqlCommand cmd3 = new SqlCommand("sp_roletype", con);
                cmd3.CommandType = CommandType.StoredProcedure;

                cmd3.Parameters.AddWithValue("@una", obj.Username);
                cmd3.Parameters.AddWithValue("@pw", obj.Password);

                con.Open();

                object role = cmd3.ExecuteScalar();

                if (role != null && role != DBNull.Value)
                {
                    result.Role = role.ToString();
                }

                con.Close();


                // Login successful
                result.IsValid = true;

                return result;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                throw new Exception("Login Error: " + ex.Message);
            }
        }
    }
}
