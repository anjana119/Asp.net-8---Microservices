using System.ComponentModel.DataAnnotations;

namespace NewMicroServices.Models
{
    public class Flight1
    {
        public int Id { get; set; }
        [Required]
        public string FlightNumber { get; set; } = string.Empty;
        [Required]
        public string DepartureCity { get; set; } = string.Empty;
        [Required]
        public string DestinationCity { get; set; } = string.Empty;
        [Required]
        public string DepartureTime { get; set; } = string.Empty;
        [Required]
        public string ArrivalTime { get; set; } = string.Empty;
        // [Required]
    }
}