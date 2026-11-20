using System.ComponentModel.DataAnnotations;

namespace Vehicle_Service.Models
{
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Vehicle Number is required")]
        public string? VehicleNumber { get; set; }

        [Required(ErrorMessage = "Vehicle Type is required")]
        public string? VehicleType { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public string? Brand { get; set; }

        [Required(ErrorMessage = "Model is required")]
        public string? Model { get; set; }

        [Required(ErrorMessage = "Fuel Type is required")]
        public string? FuelType { get; set; }
    }
}
