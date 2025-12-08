using Streamflix.Infrastructure.Entities;


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

         // Seed profiles if none exist
        if (!context.Profiles.Any())
        {
            SeedProfiles(context);
        }
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
        var profile1 = new Profile
        {
            AccountId = 1,
            Name = "Alice",
            AgeCategory = "Adult",
            ImageUrl = "https://example.com/images/alice.jpg",
            Preference = new ProfilePreference
            {
                PreferredGenres = new List<string> { "Action" },
                ContentType = "Movie",
                MinimumAge = 12,
                ContentFilters = new List<string> { "Violence", "Strong language" }
            }
        };

        var profile2 = new Profile
        {
            AccountId = 2,
            Name = "Bob",
            AgeCategory = "Teen",
            ImageUrl = "https://example.com/images/bob.jpg",
            Preference = new ProfilePreference
            {
                PreferredGenres = new List<string> { "Mystery" },
                ContentType = "Series",
                MinimumAge = 16,
                ContentFilters = new List<string> { "Violence" }
            }
        };

        context.Profiles.AddRange(profile1, profile2);
        context.SaveChanges();
    }
}
