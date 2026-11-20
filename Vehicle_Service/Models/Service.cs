namespace Vehicle_Service.Models
{
    public class Service
    {
        public int ManagerId { get; set; }
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? desc { get; set; }
        public decimal estimatedcost { get; set; }
    }
}
