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
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<Discount> Discounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Content>()
            .UseTphMappingStrategy();

        // Relationships

        // Series -> Season (One-to-Many)
        modelBuilder.Entity<Season>()
            .HasOne(s => s.Series)
            .WithMany(ser => ser.Seasons)
            .HasForeignKey(s => s.SeriesId)
            .OnDelete(DeleteBehavior.Cascade);

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
            .HasOne(p => p.Preference)
            .WithOne(pp => pp.Profile)
            .HasForeignKey<ProfilePreference>(pp => pp.ProfileId);

        // Profile -> Watchlist (One-to-Many)
        modelBuilder.Entity<Watchlist>()
            .HasOne(w => w.Profile)
            .WithMany(p => p.Watchlists)
            .HasForeignKey(w => w.ProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
