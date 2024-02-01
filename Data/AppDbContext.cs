using Microsoft.EntityFrameworkCore;
using NewMicroServices.Models;

namespace NewMicroServices.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }

        public DbSet<Flight> Flights {get;set;}

        internal async Task<HttpResponseMessage> GetAsync(string apiUrl)
        {
            throw new NotImplementedException();
        }

    }
}