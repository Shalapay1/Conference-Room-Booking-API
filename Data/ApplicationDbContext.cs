using Microsoft.EntityFrameworkCore;
using ConferenceRoomBookingAPI.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    

    public DbSet<Hall> Halls { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<HallService> HallServices { get; set; }
    public DbSet<BookingService> BookingServices { get; set; }
}
