using Microsoft.EntityFrameworkCore;
using Client.Models;

namespace Client.Data
{
    public class ClientContext : DbContext
    {
        public ClientContext(DbContextOptions<ClientContext> options) : base(options)
        {
            
        }

        public DbSet<Log> LogItems {get; set;}
    }
}