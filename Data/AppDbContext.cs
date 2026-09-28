using Microsoft.EntityFrameworkCore;
using Route = Commute360.Models.Route; 

namespace Commute360.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Commute360.Models.User> Users { get; set; }
    public DbSet<Commute360.Models.Route> Routes { get; set; }
    public DbSet<Commute360.Models.Booking> Bookings { get; set; }
}