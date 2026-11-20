using System.Data;
using System.Data.SqlClient;

namespace Vehicle_Service.Models
{

    public class BookingDB
    {
        SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true");
        // Booking methods
        public int Booking_InsertDB(Booking obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_booking_insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@cid", obj.CustomerId);
                cmd.Parameters.AddWithValue("@vid", obj.VehicleId);
                cmd.Parameters.AddWithValue("@booking_date", obj.BookingDate);
                cmd.Parameters.AddWithValue("@pbm", obj.ProblemDescription);
                cmd.Parameters.AddWithValue("@status", "Pending");

                con.Open();

                int bookingId = Convert.ToInt32(cmd.ExecuteScalar());

                con.Close();

                return bookingId;
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                    con.Close();

                return 0;
            }
        }
        public List<Vehicle> GetVehiclesDB(int customerId)
        {
            var vehicles = new List<Vehicle>();
            try
            {
                SqlCommand cmd = new SqlCommand("sp_vehicle_details", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Vehicle obj = new Vehicle();
                    {
                        obj.VehicleId = Convert.ToInt32(dr["VehicleId"]);
                        obj.VehicleNumber = dr["VehicleNumber"].ToString();
                        obj.VehicleType = dr["VehicleType"].ToString();
                        obj.Brand = dr["Brand"].ToString();
                        obj.Model = dr["Model"].ToString();
                        obj.FuelType = dr["FuelType"].ToString();
                    }
                    vehicles.Add(obj);
                }
                con.Close();
            }
            catch (Exception)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                throw;
            }
            return vehicles;
        }
        public string BookingService_InsertDB(BookingService obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_bookingservice_insert", con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@bookingid", obj.BookingId);
                cmd.Parameters.AddWithValue("@serviceid", obj.ServiceId);
               
                //here actual cost is entering from Sql procedure
                
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
            }

            return services;
        }
        public List<Booking> Booking_DetailsDB(int CustomerId)
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_booking_details",con);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@cid", CustomerId);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Booking obj = new Booking();

                    obj.BookingId =
                        Convert.ToInt32(dr["BookingId"]);

                    obj.CustomerId =
                        Convert.ToInt32(dr["CustomerId"]);

                    obj.VehicleId =
                        Convert.ToInt32(dr["VehicleId"]);

                    obj.BookingDate =
                        Convert.ToDateTime(dr["BookingDate"]);

                    obj.ProblemDescription =
                        dr["ProblemDescription"].ToString();

                    obj.Status =
                        dr["Status"].ToString();

                    bookings.Add(obj);
                }

                dr.Close();
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
