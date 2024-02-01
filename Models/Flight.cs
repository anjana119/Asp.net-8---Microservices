using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace NewMicroServices.Models
{
    public class Flight
    {
        public int Id { get; set; }
        // [Required]
        [XmlElement(ElementName = "FlightNumber")]
        public string FlightNumber { get; set; }
        [XmlElement(ElementName = "DepartureCity")]
        public string DepartureCity { get; set; }
        [XmlElement(ElementName = "DestinationCity")]
        public string DestinationCity { get; set; }
        [XmlElement(ElementName = "DepartureTime")]
        public DateTime DepartureTime { get; set; }
        [XmlElement(ElementName = "ArrivalTime")]
        public DateTime ArrivalTime { get; set; }
    }

    [XmlRoot("Flights")] 
    public class Flights
    {
        [XmlElement("Flight")]
        public Flight[] FlightList { get; set; }
    }
}