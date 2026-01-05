using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Streamflix.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // Only seed when database is empty enough to avoid duplicates
        if (!context.Movies.Any() && !context.Series.Any())
        {
            SeedMockData(context);
        }

        if (!context.Accounts.Any())
        {
            SeedAccounts(context);
        }

        // Seed profiles if none exist
        if (!context.Profiles.Any())
        {
            SeedProfiles(context);
        }

        // Seed watchlists if none exist
        if (!context.Watchlists.Any())
        {
            SeedWatchlists(context);
        }
    }

    private static void SeedAccounts(ApplicationDbContext context)
    {
        var passwordHasher = new PasswordHasher<Account>();
        var account1 = new Account
        {
            Email = "test1@test.com",
            IsActive = true,
            IsVerified = true,
            RegistrationDate = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
        account1.Password = passwordHasher.HashPassword(account1, "Wachtwoord123!");

        var account2 = new Account
        {
            Email = "test2@test.com",
            IsActive = true,
            IsVerified = true,
            RegistrationDate = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
        account2.Password = passwordHasher.HashPassword(account2, "Wachtwoord123!");

        context.Accounts.AddRange(account1, account2);
        context.SaveChanges();
    }

    private static void SeedMockData(ApplicationDbContext context)
    {
        var movie1 = new Movie
        {
            Title = "The Last Stand",
            Description = "An ex-soldier must protect a small town from a criminal gang.",
            AgeRating = 16,
            ImageURL = "https://example.com/images/the-last-stand.jpg",
            Duration = 120,

            Genre = "Action",
            ContentWarnings = new List<string> { "Violence", "Strong language" },
            AvailableQualities = new List<string> { "HD", "4K" }
        };

        var movie2 = new Movie
        {
            Title = "Laugh Out Loud",
            Description = "A group of friends navigate awkward situations with humor.",
            AgeRating = 12,
            ImageURL = "https://example.com/images/laugh-out-loud.jpg",
            Duration = 95,

            Genre = "Comedy",
            ContentWarnings = new List<string> { "Mild language" },
            AvailableQualities = new List<string> { "SD", "HD" }
        };

        var series = new Series
        {
            Title = "Mystery Manor",
            Description = "A detective unravels secrets in an old mansion.",
            AgeRating = 16,
            ImageURL = "https://example.com/images/mystery-manor.jpg",
            TotalSeasons = 1,

            Genre = "Mystery",
            ContentWarnings = new List<string> { "Violence" },
            AvailableQualities = new List<string> { "HD" },

            Seasons = new List<Season>
            {
                new Season
                {
                    SeasonNumber = 1,
                    TotalEpisodes = 3,
                    Episodes = new List<Episode>
                    {
                        new Episode { EpisodeNumber = 1, Title = "The Arrival", Duration = 45 },
                        new Episode { EpisodeNumber = 2, Title = "Hidden Room", Duration = 47 },
                        new Episode { EpisodeNumber = 3, Title = "The Truth", Duration = 50 }
                    }
                }
            }
        };

        context.Movies.AddRange(movie1, movie2);
        context.Series.Add(series);

        context.SaveChanges();
    }

    private static void SeedProfiles(ApplicationDbContext context)
    {
        var accounts = context.Accounts.ToList();
        if (accounts.Count < 2)
        {
            // Not enough accounts to seed profiles
            return;
        }
        var profile1 = new Profile
        {
            Account = accounts[0],
            Name = "Alice",
            AgeCategory = "Adult",
            ImageUrl = "https://example.com/images/alice.jpg",
            Preference = new ProfilePreference
            {
                PreferredGenres = new List<string> { "Action" },
                ContentType = "Movie",
                MinimumAge = 12,
                ContentFilters = new List<string> { "Violence", "Strong language" }
            },
            ViewingHistories = new List<ViewingHistory>
            {
                new ViewingHistory
                {
                    ContentId = 1, // for example movie1
                    EpisodeId = 0, // not applicable for movies
                    StartTime = DateTime.UtcNow.AddHours(-1),
                    EndTime = DateTime.UtcNow,
                    LastPosition = 60,
                    IsCompleted = true
                }
            }
        };

        var profile2 = new Profile
        {
            Account = accounts[1],
            Name = "Bob",
            AgeCategory = "Teen",
            ImageUrl = "https://example.com/images/bob.jpg",
            Preference = new ProfilePreference
            {
                PreferredGenres = new List<string> { "Mystery" },
                ContentType = "Series",
                MinimumAge = 16,
                ContentFilters = new List<string> { "Violence" }
            },
            ViewingHistories = new List<ViewingHistory>
            {
                new ViewingHistory
                {
                    ContentId = 3, // for example series
                    EpisodeId = 1, // first episode
                    StartTime = DateTime.UtcNow.AddHours(-2),
                    EndTime = null, // still watching
                    LastPosition = 30,
                    IsCompleted = false
                }
            }
        };

        context.Profiles.AddRange(profile1, profile2);
        context.SaveChanges();
    }

    private static void SeedWatchlists(ApplicationDbContext context)
    {
        // Ensure we have some content to add to watchlists
        var firstMovie = context.Movies.FirstOrDefault();
        var firstSeries = context.Series
            .Include(s => s.Seasons)
            .ThenInclude(se => se.Episodes)
            .FirstOrDefault();

        var profiles = context.Profiles.ToList();
        if (!profiles.Any() || (firstMovie == null && firstSeries == null))
        {
            // Nothing to seed
            return;
        }

        var profile1 = profiles.First();
        var profile2 = profiles.Count > 1 ? profiles[1] : profiles.First();

        var watchlist1 = new Watchlist
        {
            ProfileId = profile1.ProfileId,
            CreatedAt = DateTime.UtcNow,
            Items = new List<WatchlistContent>()
        };

        if (firstMovie != null)
        {
            watchlist1.Items.Add(new WatchlistContent
            {
                ContentId = firstMovie.ContentId,
                DateAdded = DateTime.UtcNow
            });
        }

        var watchlist2 = new Watchlist
        {
            ProfileId = profile2.ProfileId,
            CreatedAt = DateTime.UtcNow,
            Items = new List<WatchlistContent>()
        };

        if (firstSeries != null)
        {
            watchlist2.Items.Add(new WatchlistContent
            {
                ContentId = firstSeries.ContentId,
                DateAdded = DateTime.UtcNow
            });
        }

        context.Watchlists.AddRange(watchlist1, watchlist2);
        context.SaveChanges();
    }
}
