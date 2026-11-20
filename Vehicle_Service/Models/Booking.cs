
using System;
using System.ComponentModel.DataAnnotations;

namespace Vehicle_Service.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int CustomerId { get; set; }
        public string? CustomerName { get; set; }

        [Required(ErrorMessage = "Please select a vehicle")]
        public int VehicleId { get; set; }

        public DateTime BookingDate { get; set; }

        [Required(ErrorMessage = "Please enter the problem description")]
        public string? ProblemDescription { get; set; }

        public string? Status { get; set; }
        public string? msg { get; set; }

        // Payment status
        public string? PaymentStatus { get; set; }
    }

    public class BookingService
    {
        public int BookingServiceId { get; set; }
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public decimal ActualCost { get; set; }
    }
}
