using Hotel.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastructure.Data;

public class ToursDbContext : DbContext
{
    public ToursDbContext(DbContextOptions<ToursDbContext> options)
        : base(options)
    {
    }

    public DbSet<Departure> Departures { get; set; } = null!;
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
}