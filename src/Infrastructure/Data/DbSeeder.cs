using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Streamflix.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // Ensure the database is created, but do not migrate automatically.
        // Migrations should be handled intentionally.
        context.Database.EnsureCreated();

        // Seed non-EF managed DB objects like roles, views, SPs first.
        SeedDatabaseObjectsAndRoles(context);

        // --- Seeding Core Data ---
        // The order is important to respect foreign key constraints.
        if (!context.Accounts.Any())
        {
            SeedAccounts(context);
        }

        if (!context.Contents.Any())
        {
            SeedContent(context);
        }

        context.SaveChanges(); // Save accounts and content to get IDs for dependent entities.

        if (!context.Profiles.Any())
        {
            SeedProfiles(context);
        }

        if (!context.Subscriptions.Any())
        {
            SeedSubscriptions(context);
        }

        context.SaveChanges(); // Save profiles and subscriptions to get IDs.

        // --- Seeding Relational & Auxiliary Data ---
        if (!context.ViewingHistories.Any())
        {
            SeedViewingHistory(context);
        }

        if (!context.Watchlists.Any())
        {
            SeedWatchlists(context);
        }

        if (!context.Referrals.Any())
        {
            SeedReferrals(context);
        }

        if (!context.Discounts.Any())
        {
            SeedDiscounts(context);
        }

        context.SaveChanges(); // Final save for all remaining changes.
    }

    private static void SeedDatabaseObjectsAndRoles(ApplicationDbContext context)
    {
        // This method uses raw SQL to create database objects that are not managed by EF Core.
        // It's designed to be idempotent and uses PostgreSQL-compatible syntax.

        // 1. Internal Roles for direct DBMS access
        context.Database.ExecuteSqlRaw(@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'junior') THEN CREATE ROLE junior; END IF;
                IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'medior') THEN CREATE ROLE medior; END IF;
                IF NOT EXISTS (SELECT FROM pg_catalog.pg_roles WHERE rolname = 'senior') THEN CREATE ROLE senior; END IF;
            END;
            $$;
        ");

        // Grant permissions. Note: Case-sensitive names are quoted.
        context.Database.ExecuteSqlRaw(@"
            GRANT SELECT ON ""Accounts"", ""Profiles"" TO junior;
            GRANT SELECT, UPDATE ON ""Accounts"", ""Profiles"", ""ProfilePreferences"" TO medior;
            GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO senior;
            GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO senior;
        ");

        // 2. Views
        context.Database.ExecuteSqlRaw(@"
            DROP VIEW IF EXISTS vw_popular_content;
            CREATE VIEW vw_popular_content AS
            SELECT
                c.""Title"",
                c.""Discriminator"" AS ""ContentType"",
                c.""Genre"",
                COUNT(vh.""ViewingHistoryId"") AS ""CompletedViews""
            FROM ""Contents"" c
            JOIN ""ViewingHistories"" vh ON c.""ContentId"" = vh.""ContentId""
            WHERE vh.""IsCompleted"" = true
            GROUP BY c.""Title"", c.""Discriminator"", c.""Genre"";
        ");

        // 3. Functions (instead of Stored Procedures for returning data in PostgreSQL)
        context.Database.ExecuteSqlRaw(@"
            CREATE OR REPLACE FUNCTION fn_get_profile_watchlist(p_profile_id INT)
            RETURNS TABLE (
                ""Title"" TEXT,
                ""Description"" TEXT,
                ""Genre"" TEXT,
                ""DateAdded"" TIMESTAMPTZ
            )
            LANGUAGE plpgsql
            AS $$
            BEGIN
                RETURN QUERY
                SELECT c.""Title"", c.""Description"", c.""Genre"", wc.""DateAdded""
                FROM ""WatchlistContents"" wc
                JOIN ""Contents"" c ON wc.""ContentId"" = c.""ContentId""
                JOIN ""Watchlists"" w ON wc.""WatchlistId"" = w.""WatchlistId""
                WHERE w.""ProfileId"" = p_profile_id
                ORDER BY wc.""DateAdded"" DESC;
            END;
            $$;
        ");

        // 4. Triggers
        context.Database.ExecuteSqlRaw(@"
            CREATE OR REPLACE FUNCTION fn_prevent_active_subscription_deletion()
            RETURNS TRIGGER
            LANGUAGE plpgsql
            AS $$
            BEGIN
                IF OLD.""IsActive"" = true AND OLD.""EndDate"" > (NOW() AT TIME ZONE 'UTC') THEN
                    RAISE EXCEPTION 'Active subscriptions cannot be deleted. Deactivate them first.';
                END IF;
                RETURN OLD; -- Allow deletion if condition is not met
            END;
            $$;
        ");
        context.Database.ExecuteSqlRaw(@"
            DROP TRIGGER IF EXISTS trg_prevent_active_subscription_deletion ON ""Subscriptions"";
            CREATE TRIGGER trg_prevent_active_subscription_deletion
            BEFORE DELETE ON ""Subscriptions""
            FOR EACH ROW
            EXECUTE FUNCTION fn_prevent_active_subscription_deletion();
        ");
    }

    private static void SeedAccounts(ApplicationDbContext context)
    {
        var passwordHasher = new PasswordHasher<Account>();
        var accounts = new List<Account>();

        for (int i = 1; i <= 10; i++)
        {
            var account = new Account
            {
                Email = $"user{i}@streamflix.com",
                IsActive = i % 3 != 0, // Some inactive accounts
                IsVerified = i % 4 != 0, // Some unverified accounts
                RegistrationDate = DateTime.UtcNow.AddDays(-i * 10),
                LastLogin = DateTime.UtcNow.AddDays(-i)
            };
            account.Password = passwordHasher.HashPassword(account, "Password123!");
            accounts.Add(account);
        }
        context.Accounts.AddRange(accounts);
    }

    private static void SeedContent(ApplicationDbContext context)
    {
        var movies = new List<Movie>
        {
            new Movie { Title = "Stellar Odyssey", Description = "A journey to the edge of the universe.", AgeRating = 12, ImageURL = "/img/stellar_odyssey.jpg", Duration = 148, Genre = "Sci-Fi", ContentWarnings = new List<string> { "Peril" }, AvailableQualities = new List<string> { "HD", "4K" } },
            new Movie { Title = "Code of the Samurai", Description = "A tale of honor and rebellion in feudal Japan.", AgeRating = 18, ImageURL = "/img/code_samurai.jpg", Duration = 162, Genre = "Action", ContentWarnings = new List<string> { "Violence" }, AvailableQualities = new List<string> { "4K" } },
            new Movie { Title = "The Last Laugh", Description = "A washed-up comedian gets a second chance.", AgeRating = 16, ImageURL = "/img/last_laugh.jpg", Duration = 105, Genre = "Comedy", ContentWarnings = new List<string> { "Language" }, AvailableQualities = new List<string> { "HD" } },
            new Movie { Title = "Beneath the Surface", Description = "A deep-sea documentary.", AgeRating = 0, ImageURL = "/img/beneath_surface.jpg", Duration = 90, Genre = "Documentary", ContentWarnings = new List<string>(), AvailableQualities = new List<string> { "SD", "HD" } }
        };

        var series = new List<Series>
        {
            new Series
            {
                Title = "The Crown of Kings",
                Description = "A medieval fantasy epic of power and betrayal.",
                AgeRating = 18,
                ImageURL = "/img/crown_kings.jpg",
                TotalSeasons = 2,
                Genre = "Fantasy",
                ContentWarnings = new List<string> { "Violence", "Nudity" },
                AvailableQualities = new List<string> { "HD", "4K" },
                Seasons = new List<Season>
                {
                    new Season { SeasonNumber = 1, TotalEpisodes = 5, Episodes = new List<Episode> {
                        new Episode { EpisodeNumber = 1, Title = "The Throne of Thorns", Duration = 55 },
                        new Episode { EpisodeNumber = 2, Title = "The Whispering Woods", Duration = 58 },
                        new Episode { EpisodeNumber = 3, Title = "A Pact of Fire", Duration = 56 },
                        new Episode { EpisodeNumber = 4, Title = "The Dragon's Due", Duration = 62 },
                        new Episode { EpisodeNumber = 5, Title = "The Long Night", Duration = 65 }
                    }},
                    new Season { SeasonNumber = 2, TotalEpisodes = 5, Episodes = new List<Episode> {
                        new Episode { EpisodeNumber = 1, Title = "A Kingdom Divided", Duration = 57 },
                        new Episode { EpisodeNumber = 2, Title = "The Serpent's Kiss", Duration = 59 },
                        new Episode { EpisodeNumber = 3, Title = "March of the Damned", Duration = 61 },
                        new Episode { EpisodeNumber = 4, Title = "The Siege of Silverfall", Duration = 64 },
                        new Episode { EpisodeNumber = 5, Title = "Winter's Heart", Duration = 70 }
                    }}
                }
            },
            new Series
            {
                Title = "Cybernetic Dawn",
                Description = "In a dystopian future, a group of rebels fight a sentient AI.",
                AgeRating = 16,
                ImageURL = "/img/cyber_dawn.jpg",
                TotalSeasons = 1,
                Genre = "Sci-Fi",
                ContentWarnings = new List<string> { "Violence", "Language" },
                AvailableQualities = new List<string> { "HD" },
                Seasons = new List<Season>
                {
                    new Season { SeasonNumber = 1, TotalEpisodes = 8, Episodes = new List<Episode> {
                        new Episode { EpisodeNumber = 1, Title = "The Awakening", Duration = 45 },
                        new Episode { EpisodeNumber = 2, Title = "Ghost in the Machine", Duration = 47 },
                        new Episode { EpisodeNumber = 3, Title = "The Glitch", Duration = 46 },
                        new Episode { EpisodeNumber = 4, Title = "Digital Revolution", Duration = 48 },
                        new Episode { EpisodeNumber = 5, Title = "Firewall Breach", Duration = 49 },
                        new Episode { EpisodeNumber = 6, Title = "Sovereign", Duration = 52 },
                        new Episode { EpisodeNumber = 7, Title = "The Human Factor", Duration = 51 },
                        new Episode { EpisodeNumber = 8, Title = "Reboot", Duration = 55 }
                    }}
                }
            }
        };

        context.Movies.AddRange(movies);
        context.Series.AddRange(series);
    }

    private static void SeedProfiles(ApplicationDbContext context)
    {
        var accounts = context.Accounts.ToList();
        var profiles = new List<Profile>();
        var profileNames = new[] { "Main", "Kids", "Guest", "Teen" };
        var ageCategories = new[] { "Adult", "Child", "Adult", "Teen" };

        foreach (var account in accounts)
        {
            for (int i = 0; i < 4; i++)
            {
                profiles.Add(new Profile
                {
                    Account = account,
                    Name = profileNames[i],
                    AgeCategory = ageCategories[i],
                    ImageUrl = $"/img/avatar_{i+1}.png",
                    Preference = new ProfilePreference
                    {
                        PreferredGenres = new List<string> { (i % 2 == 0) ? "Sci-Fi" : "Comedy" },
                        ContentType = "Any",
                        MinimumAge = (ageCategories[i] == "Child") ? 0 : 16
                    }
                });
            }
        }
        context.Profiles.AddRange(profiles);
    }

    private static void SeedSubscriptions(ApplicationDbContext context)
    {
        var accounts = context.Accounts.Where(a => a.IsActive).ToList();
        var subscriptions = new List<Subscription>();
        var subTypes = new[] { "Basic", "Standard", "Premium" };
        var prices = new[] { 9.99, 15.99, 21.99 };
        var descriptions = new[] { "Basic SD streaming plan", "Standard HD streaming plan", "Premium 4K+HDR streaming plan" };

        for (int i = 0; i < accounts.Count; i++)
        {
            var acc = accounts[i];
            var subTypeIndex = i % subTypes.Length;
            subscriptions.Add(new Subscription
            {
                Account = acc,
                SubscriptionType = subTypes[subTypeIndex],
                SubscriptionDescription = descriptions[subTypeIndex],
                BasePrice = prices[subTypeIndex],
                StartDate = acc.RegistrationDate,
                EndDate = acc.RegistrationDate.AddYears(1),
                IsActive = true
            });
        }
        context.Subscriptions.AddRange(subscriptions);
    }

    private static void SeedViewingHistory(ApplicationDbContext context)
    {
        var profiles = context.Profiles.Include(p => p.Account).ToList();
        var movies = context.Movies.ToList();
        var seriesEpisodes = context.Episodes.Include(e => e.Season).ThenInclude(s => s.Series).ToList();
        var history = new List<ViewingHistory>();
        var random = new Random();

        foreach (var profile in profiles)
        {
            // Watch some movies
            for (int i = 0; i < 2; i++)
            {
                var movie = movies[random.Next(movies.Count)];
                var isCompleted = random.Next(2) == 0;
                history.Add(new ViewingHistory
                {
                    Profile = profile,
                    ContentId = movie.ContentId,
                    StartTime = DateTime.UtcNow.AddDays(-random.Next(1, 20)),
                    EndTime = isCompleted ? DateTime.UtcNow.AddDays(-random.Next(1, 20)).AddMinutes(movie.Duration) : (DateTime?)null,
                    LastPosition = isCompleted ? movie.Duration : random.Next(1, movie.Duration -1),
                    IsCompleted = isCompleted
                });
            }

            // Watch some series episodes
            for (int i = 0; i < 3; i++)
            {
                var episode = seriesEpisodes[random.Next(seriesEpisodes.Count)];
                var isCompleted = random.Next(2) == 0;
                history.Add(new ViewingHistory
                {
                    Profile = profile,
                    ContentId = episode.Season.Series.ContentId,
                    EpisodeId = episode.EpisodeId,
                    StartTime = DateTime.UtcNow.AddDays(-random.Next(1, 20)),
                    EndTime = isCompleted ? DateTime.UtcNow.AddDays(-random.Next(1, 20)).AddMinutes(episode.Duration) : (DateTime?)null,
                    LastPosition = isCompleted ? episode.Duration : random.Next(1, episode.Duration - 1),
                    IsCompleted = isCompleted
                });
            }
        }
        context.ViewingHistories.AddRange(history);
    }

    private static void SeedWatchlists(ApplicationDbContext context)
    {
        var profiles = context.Profiles.ToList();
        var content = context.Contents.ToList();
        var watchlists = new List<Watchlist>();
        var random = new Random();

        foreach (var profile in profiles)
        {
            var watchlist = new Watchlist
            {
                Profile = profile,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                Items = new List<WatchlistContent>()
            };

            for (int i = 0; i < 5; i++)
            {
                watchlist.Items.Add(new WatchlistContent
                {
                    Content = content[random.Next(content.Count)],
                    DateAdded = DateTime.UtcNow.AddDays(-random.Next(1, 30))
                });
            }
            watchlists.Add(watchlist);
        }
        context.Watchlists.AddRange(watchlists);
    }

    private static void SeedReferrals(ApplicationDbContext context)
    {
        var accounts = context.Accounts.ToList();
        if (accounts.Count < 2) return;

        var referral = new Referral
        {
            ReferrerAccount = accounts[0],
            ReferredAccount = accounts[1],
            InvitationCode = Guid.NewGuid().ToString("N").Substring(0, 8),
            InvitationDate = DateTime.UtcNow.AddDays(-5),
            AcceptDate = DateTime.UtcNow.AddDays(-4),
            IsDiscountApplied = true,
        };
        context.Referrals.Add(referral);
    }



    private static void SeedDiscounts(ApplicationDbContext context)
    {
        var accountWithReferral = context.Accounts.FirstOrDefault(a => a.SentReferrals.Any());
        if (accountWithReferral == null) return;

        var discount = new Discount
        {
            Account = accountWithReferral,
            DiscountAmount = 5.00, // 5 euro discount for successful referral
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true
        };
        context.Discounts.Add(discount);
    }
}
