namespace VehicleService.Messaging
{
    public class BookingCreatedMessage
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string VehicleDescription { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string ProblemDescription { get; set; } = string.Empty;
    }
}