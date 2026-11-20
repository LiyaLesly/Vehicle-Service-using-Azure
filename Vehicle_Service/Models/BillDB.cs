using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Data;
using System.Data.SqlClient;

namespace Vehicle_Service.Models
{
    public class BillDB
    {
        SqlConnection con = new SqlConnection(@"server=LAPTOP-3M9LC1FT\SQLEXPRESS;database=Vehicle_mvc_core;integrated security=true");

        public string Bill_InsertDB(Bill obj)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_bill_insert", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@cid", obj.CustomerId);
                cmd.Parameters.AddWithValue("@bid", obj.BookingId);
                cmd.Parameters.AddWithValue("@status", "Pending");

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return "Bill Generated Successfully";
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
        public List<Bill> Customer_Bill_DetailsDB(int customerId)
        {
            List<Bill> bills = new List<Bill>();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_customer_bill", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@cid", customerId);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Bill obj = new Bill();
                    {
                        obj.BillId = Convert.ToInt32(dr["BillId"]);
                        obj.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                        obj.BookingId = Convert.ToInt32(dr["BookingId"]);
                        obj.Billdate = Convert.ToDateTime(dr["BillDate"]);
                        obj.totalamount = Convert.ToDecimal(dr["TotalAmount"]);
                        obj.status = dr["Status"].ToString();
                    }
                    bills.Add(obj);
                }

                con.Close();
            }
            catch
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }

            return bills;
        }

        public string Bill_PayDB(int billId, int customerId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("sp_UpdateBillStatus", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@BillId", billId);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                con.Open();

                int rowsAffected = Convert.ToInt32(cmd.ExecuteScalar());

                con.Close();

                if (rowsAffected > 0)
                {
                    return "Payment Successful";
                }
                else
                {
                    return "Bill not found or already paid";
                }
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

    }
}
