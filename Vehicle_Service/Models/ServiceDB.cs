using System.Data;
using System.Data.SqlClient;

namespace Vehicle_Service.Models
{
    public class ServiceDB
    {
        SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true");

        public string Service_InsertDB(Service clsobj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_service_insert", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@sname", clsobj.ServiceName);
                cmd.Parameters.AddWithValue("@des", clsobj.desc);
                cmd.Parameters.AddWithValue("@cost", clsobj.estimatedcost);
               
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return "Service Added Successfully Inserted";
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

                return ex.Message;
            }
        }
        public List<Service> Service_DetailsDB()
        {
            List<Service> services = new List<Service>();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_service_details", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Service obj = new Service();
                    {
                        obj.ServiceId = Convert.ToInt32(dr["ServiceId"]);
                        obj.ServiceName = dr["ServiceName"].ToString();
                        obj.desc = dr["Description"].ToString();
                        obj.estimatedcost = Convert.ToDecimal(dr["EstimatedCost"]);
                    }
                    services.Add(obj);
                }
                con.Close();
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                throw;   
            }

            return services;
        }
        public List<Booking> Manager_Booking_ViewDB()
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_manager_booking_view", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Booking obj = new Booking();

                    obj.BookingId = Convert.ToInt32(dr["BookingId"]);
                    obj.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                    obj.CustomerName = dr["CustomerName"].ToString();
                    obj.VehicleId = Convert.ToInt32(dr["VehicleId"]);
                    obj.BookingDate = Convert.ToDateTime(dr["BookingDate"]);
                    obj.ProblemDescription = dr["ProblemDescription"].ToString();
                    obj.Status = dr["Status"].ToString();

                    bookings.Add(obj);
                }
                con.Close();
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                throw;
            }

            return bookings;
        }
        public List<Booking> Manager_Bill_ViewDB()
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_manager_bill_view", con);
                cmd.CommandType = CommandType.StoredProcedure;

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Booking obj = new Booking();

                    obj.BookingId = Convert.ToInt32(dr["BookingId"]);
                    obj.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                    obj.CustomerName = dr["CustomerName"].ToString();
                    obj.VehicleId = Convert.ToInt32(dr["VehicleId"]);
                    obj.BookingDate = Convert.ToDateTime(dr["BookingDate"]);
                    obj.ProblemDescription = dr["ProblemDescription"].ToString();
                    obj.Status = dr["Status"].ToString();

                    // Retrieve payment status
                    obj.PaymentStatus = dr["PaymentStatus"] == DBNull.Value
                        ? null
                        : dr["PaymentStatus"].ToString();

                    bookings.Add(obj);
                }

                con.Close();
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return bookings;
        }
    }
}
