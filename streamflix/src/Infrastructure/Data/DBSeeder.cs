using Streamflix.Infrastructure.Entities;

namespace Streamflix.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Ensure DB exists
        await context.Database.EnsureCreatedAsync();

        // === SEED GENRES ===
        if (!context.Genres.Any())
        {
            context.Genres.AddRange(
                new Genre { GenreType = "Action" },
                new Genre { GenreType = "Drama" },
                new Genre { GenreType = "Comedy" }
            );
        }

        // === SEED QUALITIES ===
        if (!context.Qualities.Any())
        {
            context.Qualities.AddRange(
                new Quality { QualityType = "HD" },
                new Quality { QualityType = "FullHD" },
                new Quality { QualityType = "4K" }
            );
        }

        // === SEED CONTENT WARNINGS ===
        if (!context.ContentWarnings.Any())
        {
            context.ContentWarnings.AddRange(
                new ContentWarning { ContentWarningType = "Violence" },
                new ContentWarning { ContentWarningType = "Language" },
                new ContentWarning { ContentWarningType = "Horror" }
            );
        }

        await context.SaveChangesAsync();
    }
}
