using Microsoft.EntityFrameworkCore;
using ContractsWebApp.Models;

namespace ContractsWebApp.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Participant> Participants { get; set; }
    }
}
