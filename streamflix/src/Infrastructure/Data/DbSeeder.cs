using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Entities;
using Streamflix.Infrastructure.Enums;

namespace Streamflix.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Seed Genres
        if (!await context.Genres.AnyAsync())
        {
            var genres = Enum.GetValues<GenreType>()
                .Select(g => new Genre { GenreType = g })
                .ToList();
            
            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();
        }

        // Seed Qualities
        if (!await context.Qualities.AnyAsync())
        {
            var qualities = Enum.GetValues<QualityType>()
                .Select(q => new Quality { QualityType = q })
                .ToList();

            await context.Qualities.AddRangeAsync(qualities);
            await context.SaveChangesAsync();
        }

        // Seed ContentWarnings
        if (!await context.ContentWarnings.AnyAsync())
        {
            var warnings = Enum.GetValues<ContentWarningType>()
                .Select(w => new ContentWarning { ContentWarningType = w })
                .ToList();

            await context.ContentWarnings.AddRangeAsync(warnings);
            await context.SaveChangesAsync();
        }
    }
}
