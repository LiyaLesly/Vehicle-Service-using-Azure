namespace Vehicle_Service.Models
{
    public class Bill
    {
        public int BillId { get; set; }
        public int CustomerId { get; set; }
        public int BookingId { get; set; }
        public DateTime Billdate { get; set; }
        public decimal totalamount { get; set; }
        public string? status { get; set; }
    } 
}
