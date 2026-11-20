using System.Data;
using System.Data.SqlClient;
using Vehicle_Service.Models;


namespace Vehicle_Service.Models
{
    public class VehicleDB
    {
        SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true");

        public string Vehicle_InsertDB(Vehicle clsobj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_vehicle_insert", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Cid", clsobj.CustomerId);
                cmd.Parameters.AddWithValue("@Veh_no", clsobj.VehicleNumber);
                cmd.Parameters.AddWithValue("@Veh_type", clsobj.VehicleType);
                cmd.Parameters.AddWithValue("@brand", clsobj.Brand);
                cmd.Parameters.AddWithValue("@model", clsobj.Model);
                cmd.Parameters.AddWithValue("@fuel_type", clsobj.FuelType);

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
                return "Vehicle Insert Error: " + ex.Message;
            }
        }
        // GET CUSTOMER VEHICLES
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
    }
}


//public → this method can be called from another class, such as VehicleController.
//List<Vehicle> → the method will return a list of Vehicle objects.
//GetVehicles → method name.
//int customerId → receives the customer's ID as input.

//GetVehicles(int customerId) takes a Customer ID, calls the stored procedure, 
//reads that customer's vehicles one by one, converts each database row into a Vehicle object,
//stores them in a list, and returns that list to the controller.


//this is another method to add vehicles

//vehicles.Add(new Vehicle
//{
//    VehicleId = Convert.ToInt32(dr["VehicleId"]),
//    VehicleNumber = dr["VehicleNumber"].ToString(),
//    VehicleType = dr["VehicleType"].ToString(),
//    Brand = dr["Brand"].ToString(),
//    Model = dr["Model"].ToString(),
//    FuelType = dr["FuelType"].ToString()
//});