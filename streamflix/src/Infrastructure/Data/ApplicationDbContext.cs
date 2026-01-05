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
    public DbSet<ViewingHistory> ViewingHistories { get; set; }

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

        modelBuilder.Entity<Referral>()
            .HasKey(r => r.ReferralId);

        modelBuilder.Entity<Discount>()
            .HasKey(d => d.DiscountId);

        // Profile -> ViewingHistory (One-to-Many)
        modelBuilder.Entity<Profile>()
            .HasMany(p => p.ViewingHistories)
            .WithOne(vh => vh.Profile)
            .HasForeignKey(vh => vh.ProfileId);

        // Account -> Profile (One-to-Many)
        modelBuilder.Entity<Account>()
            .HasMany(a => a.Profiles)
            .WithOne(p => p.Account)
            .HasForeignKey(p => p.AccountId);

        // Account -> Subscription (One-to-One)
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Subscription)
            .WithOne(s => s.Account)
            .HasForeignKey<Subscription>(s => s.AccountId);

        // Account -> Discount (One-to-Many)
        modelBuilder.Entity<Account>()
            .HasMany(a => a.Discounts)
            .WithOne(d => d.Account)
            .HasForeignKey(d => d.AccountId);

        // Referral relationships
        modelBuilder.Entity<Referral>()
            .HasOne(r => r.ReferrerAccount)
            .WithMany(a => a.SentReferrals)
            .HasForeignKey(r => r.ReferrerAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Referral>()
            .HasOne(r => r.ReferredAccount)
            .WithMany(a => a.ReceivedReferrals)
            .HasForeignKey(r => r.ReferredAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Profile -> Watchlist (One-to-One)
        modelBuilder.Entity<Profile>()
            .HasOne(p => p.Watchlist)
            .WithOne(w => w.Profile)
            .HasForeignKey<Watchlist>(w => w.ProfileId);
    }
}
