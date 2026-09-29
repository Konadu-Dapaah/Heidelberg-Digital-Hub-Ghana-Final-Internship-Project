// imports statements
using Microsoft.EntityFrameworkCore;
using Route = Commute360.Models.Route; 

namespace Commute360.Data;
public class AppDbContext : DbContext
{
    // constructor that takes DbContextOptions and passes it to the base class constructor
    //as well as ccreates database tables for stops, users, routes and bookings
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Commute360.Models.Stop> Stops { get; set; }
    public DbSet<Commute360.Models.User> Users { get; set; }
    public DbSet<Commute360.Models.Route> Routes { get; set; }
    public DbSet<Commute360.Models.Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Commute360.Models.Booking>()
        .HasOne(b => b.BoardingStop)
        .WithMany()
        .HasForeignKey(b => b.BoardingStopId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Commute360.Models.Booking>()
        .HasOne(b => b.DropOffStop)
        .WithMany()
        .HasForeignKey(b => b.DropOffStopId)
        .OnDelete(DeleteBehavior.Restrict);
}
}