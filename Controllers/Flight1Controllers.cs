using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewMicroServices.Data;
using NewMicroServices.Models;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace NewMicroServices.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class Flight_Local_DB_Controllers:ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;


        public Flight_Local_DB_Controllers(AppDbContext context)
        {
            _context=context;
            _httpClient = new HttpClient();
        }


        // Get Flights XML Format
        [HttpGet("Get Flights XML Format")]
        public async Task<IActionResult> GetDataXML()
        {
            // Fetch details from provided API.
            var apiUrl = "https://flighttestservice.azurewebsites.net/flights";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return Ok(data);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }



        // Get Flights JSON Format
        [HttpGet("Get Flights JSON Format")]
        public async Task<IActionResult> GetDataJSON()
        {
            var apiUrl = "https://flighttestservice.azurewebsites.net/flights";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                
                // Prase XML formate to JSON object model.
                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.LoadXml(data);
                string json = JsonConvert.SerializeXmlNode(xmlDoc, Formatting.Indented);
                return Ok(json);
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }



        // Auto Save Flights JSON Format
        [HttpPost("Saving All Flights")]
        public async Task<IActionResult> GetFlightData()
        {
            var apiUrl = "https://flighttestservice.azurewebsites.net/flights";
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();

                XmlSerializer serializer = new XmlSerializer(typeof(Flights));
                using StringReader reader = new StringReader(data);
                var flights = (Flights)serializer.Deserialize(reader);
                
                foreach(var item in flights.FlightList) {
                    var flight = new Flight{
                        FlightNumber = item.FlightNumber,
                        DepartureCity = item.DepartureCity,
                        DestinationCity = item.DestinationCity,
                        DepartureTime = item.DepartureTime,
                        ArrivalTime = item.ArrivalTime
                    };
                    await _context.AddAsync(flight);
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0) {
                    return Ok("Sucessfully Saved!");
                }
                return BadRequest("Unable to create record!");
            }
            else
            {
                return StatusCode((int)response.StatusCode);
            }
        }


        // Get all method
        [HttpGet("Get All Records")]
        public async Task<IEnumerable<Flight>> GetFlights()
        {
            var flights = await _context.Flights.AsNoTracking().ToListAsync();

            return flights;
        }



        // Post method
        [HttpPost]
        public async Task<IActionResult> Create(Flight flight)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.AddAsync(flight);

            var result = await _context.SaveChangesAsync();

            if (result > 0) {
                return Ok("Sucessfully record saved!");
            }

            return BadRequest("Unable to create record!");
        }



        // Get/Filter by method (-- Record ID --)
        [HttpGet("Filter By Record Id/{id}")]
        public async Task<IActionResult> GetFlight(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if(flight is null)
                return NotFound("Unable to find record!");

            return Ok(flight);
        }



        // Get/Filter by method (-- Flight Number --)
        [HttpGet("Filter By Flight Number/{flightNumber}")]
        public IActionResult GetFlightByFlightNumber(string flightNumber)
        {
            var flight = _context.Flights.Where(x => x.FlightNumber == flightNumber);
            if (flight is null)
            {
                return NotFound("Unable to find flight details!");
            }
            return Ok(flight);
        }



        // Get/Filter by method (-- DepartureCity --)
        [HttpGet("Filter By Departure City/{departureCity}")]
        public IActionResult GetFlightByDepartureCity(string departureCity)
        {
            var flight = _context.Flights.Where(x => x.DepartureCity == departureCity);
            if (flight is null)
            {
                return NotFound("Unable to find flight details!");
            }
            return Ok(flight);
        }



        // Get/Filter by method (-- ArrivalTime --)
        [HttpGet("Filter By Arrival Date & Time/{arrivalTime}")]
        public IActionResult GetFlightByArrivalTime(DateTime arrivalTime)
        {
            var flight = _context.Flights.Where(x => x.ArrivalTime == arrivalTime);
            if (flight is null)
            {
                return NotFound("Unable to find flight details!");
            }
            return Ok(flight);
        }



        // Get/Filter by method (-- DepartureCity , DestinationCity --)
        [HttpGet("Filter By Departure & Destination/{departureCity}")]
        public IActionResult GetFlightByDestination(string departureCity, string destinationCity)
        {
            var flight = _context.Flights.Where(x => x.DepartureCity == departureCity || x.DestinationCity == destinationCity);
            if (flight is null)
            {
                return NotFound("Unable to find flight details!");
            }
            return Ok(flight);
        }



        // Get/Filter by method (-- DepartureTime , ArrivalTime --)
        [HttpGet("Filter By Departure, Arrival Date & Time/{departureTime}")]
        public IActionResult GetFlightByDate(DateTime departureTime, DateTime arrivalTime)
        {
            var flight = _context.Flights.Where(x => x.DepartureTime == departureTime || x.ArrivalTime == arrivalTime);
            if (flight is null)
            {
                return NotFound("Unable to find flight details!");
            }
            return Ok(flight);
        }



        // Delete method
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if(flight is null)
                return NotFound("No records belongs to that ID!");
            _context.Remove(flight);

            var result = await _context.SaveChangesAsync();
            if(result > 0)
                return Ok("Record Deleted!");
            
            return BadRequest("Unable to delete record!");
        }



        // Update Method
        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditFlight(int id, Flight flight)
        {
            var selectedFlight = await _context.Flights.FindAsync(id);

            if(selectedFlight is null)
            {
                return BadRequest("Flight not fount!");
            }

            selectedFlight.FlightNumber = flight.FlightNumber;
            selectedFlight.DepartureCity = flight.DepartureCity;
            selectedFlight.DestinationCity = flight.DestinationCity;
            selectedFlight.DepartureTime = flight.DepartureTime;
            selectedFlight.ArrivalTime = flight.ArrivalTime;

            var result = await _context.SaveChangesAsync();

            if(result > 0)
            {
                return Ok("Flight details sucessfully updated!");
            }

            return BadRequest("Unable to update flight data!");
        }


    }

}