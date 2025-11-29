using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Content> Contents { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Season> Seasons { get; set; }

    // ADD THIS LINE
    public DbSet<Account> Accounts { get; set; }  // <-- required
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure inheritance for Content
        modelBuilder.Entity<Content>()
            .UseTphMappingStrategy();

        // Configure relationships
        
        // Series -> Season (One-to-Many)
        modelBuilder.Entity<Season>()
            .HasOne(s => s.Series)
            .WithMany(ser => ser.Seasons)
            .HasForeignKey(s => s.SeriesId);

        // Season -> Episode (One-to-Many)
        modelBuilder.Entity<Episode>()
            .HasOne(e => e.Season)
            .WithMany(s => s.Episodes)
            .HasForeignKey(e => e.SeasonId);
    }
}
