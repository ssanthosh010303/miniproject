using Microsoft.EntityFrameworkCore;

using WebApi.Models;

namespace WebApi.Repositories;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Cast> Casts { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<MovieShow> MovieShows { get; set; }
    public DbSet<Theater> Theaters { get; set; }
    public DbSet<Promo> Promos { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Screen> Screens { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<SeatType> SeatTypes { get; set; }
    public DbSet<Facility> Facilities { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<User> Users { get; set; }
}
