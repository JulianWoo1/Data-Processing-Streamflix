using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for entities \\
    // Content
    public DbSet<Content> Contents { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Series> Series { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Season> Seasons { get; set; }

    // Watchlist
    public DbSet<Watchlist> Watchlists { get; set; }
    public DbSet<WatchlistContent> WatchlistContents { get; set; }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ProfilePreference> ProfilePreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure inheritance for Content
        modelBuilder.Entity<Content>()
            .UseTphMappingStrategy(); // Table-per-Hierarchy is default, but good to be explicit or choose TPT if preferred.

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

        // Watchlist -> WatchlistContent (One-to-Many)
        modelBuilder.Entity<WatchlistContent>()
            .HasOne(wc => wc.Watchlist)
            .WithMany(w => w.Items)
            .HasForeignKey(wc => wc.WatchlistId);

        // Content -> WatchlistContent (One-to-Many)
        modelBuilder.Entity<WatchlistContent>()
            .HasOne(wc => wc.Content)
            .WithMany()
            .HasForeignKey(wc => wc.ContentId);

        // Profile -> ProfilePreference (One-to-One)
        modelBuilder.Entity<Profile>()
            .HasOne(p => p.Preferences)
            .WithOne(pp => pp.Profile)
            .HasForeignKey<ProfilePreference>(pp => pp.ProfileId);
    }
}
