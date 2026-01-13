using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Services
{
    public interface IContentService
    {
        Task<IEnumerable<Movie>> GetMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task<Movie?> GetMovieByTitleAsync(string title);
        Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genre);
        Task<Movie> CreateMovieAsync(MovieRequestDto movieDto);
        Task<Movie?> UpdateMovieAsync(int id, MovieRequestDto movieDto);
        Task<bool> DeleteMovieAsync(int id);

        Task<IEnumerable<Series>> GetSeriesAsync();
        Task<Series?> GetSeriesByIdAsync(int id);
        Task<Series?> GetSeriesByTitleAsync(string title);
        Task<IEnumerable<Series>> GetSeriesByGenreAsync(string genre);
        Task<Series> CreateSeriesAsync(SeriesRequestDto seriesDto);
        Task<Series?> UpdateSeriesAsync(int id, SeriesRequestDto seriesDto);
        Task<bool> DeleteSeriesAsync(int id);

        Task<PersonalizedMoviesResult> GetPersonalizedMoviesAsync(int profileId);
        Task<PersonalizedSeriesResult> GetPersonalizedSeriesAsync(int profileId);
    }

    public class PersonalizedMoviesResult
    {
        public bool ProfileExists { get; set; }
        public List<Movie> Movies { get; set; } = new();
    }

    public class PersonalizedSeriesResult
    {
        public bool ProfileExists { get; set; }
        public List<Series> Series { get; set; } = new();
    }

    public class ContentService : IContentService
    {
        private readonly ApplicationDbContext _db;

        public ContentService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync()
        {
            return await _db.Movies.ToListAsync();
        }

        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _db.Movies
                .FirstOrDefaultAsync(m => m.ContentId == id);
        }

        public async Task<Movie?> GetMovieByTitleAsync(string title)
        {
            var normalizedTitle = title.ToLower();
            return await _db.Movies
                .FirstOrDefaultAsync(m => m.Title.ToLower() == normalizedTitle);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByGenreAsync(string genre)
        {
            var normalizedGenre = genre.ToLower();
            return await _db.Movies
                .Where(m => m.Genre.ToLower() == normalizedGenre)
                .ToListAsync();
        }

        public async Task<Movie> CreateMovieAsync(MovieRequestDto movieDto)
        {
            var movie = new Movie
            {
                Title = movieDto.Title,
                Description = movieDto.Description,
                AgeRating = movieDto.AgeRating,
                ImageURL = movieDto.ImageURL,
                Duration = movieDto.Duration,
                Genre = movieDto.Genre,
                ContentWarnings = movieDto.ContentWarnings,
                AvailableQualities = movieDto.AvailableQualities
            };

            _db.Movies.Add(movie);
            await _db.SaveChangesAsync();

            return movie;
        }

        public async Task<Movie?> UpdateMovieAsync(int id, MovieRequestDto movieDto)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
            {
                return null;
            }

            movie.Title = movieDto.Title;
            movie.Description = movieDto.Description;
            movie.AgeRating = movieDto.AgeRating;
            movie.ImageURL = movieDto.ImageURL;
            movie.Duration = movieDto.Duration;
            movie.Genre = movieDto.Genre;
            movie.ContentWarnings = movieDto.ContentWarnings;
            movie.AvailableQualities = movieDto.AvailableQualities;

            await _db.SaveChangesAsync();
            return movie;
        }

        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
            {
                return false;
            }

            _db.Movies.Remove(movie);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Series>> GetSeriesAsync()
        {
            return await _db.Series
                .Include(s => s.Seasons)
                    .ThenInclude(season => season.Episodes)
                .ToListAsync();
        }

        public async Task<Series?> GetSeriesByIdAsync(int id)
        {
            return await _db.Series
                .Include(s => s.Seasons)
                    .ThenInclude(season => season.Episodes)
                .FirstOrDefaultAsync(s => s.ContentId == id);
        }

        public async Task<Series?> GetSeriesByTitleAsync(string title)
        {
            var normalizedTitle = title.ToLower();
            return await _db.Series
                .Include(s => s.Seasons)
                    .ThenInclude(season => season.Episodes)
                .FirstOrDefaultAsync(s => s.Title.ToLower() == normalizedTitle);
        }

        public async Task<IEnumerable<Series>> GetSeriesByGenreAsync(string genre)
        {
            var normalizedGenre = genre.ToLower();
            return await _db.Series
                .Include(s => s.Seasons)
                    .ThenInclude(season => season.Episodes)
                .Where(s => s.Genre.ToLower() == normalizedGenre)
                .ToListAsync();
        }

        public async Task<Series> CreateSeriesAsync(SeriesRequestDto seriesDto)
        {
            var series = new Series
            {
                Title = seriesDto.Title,
                Description = seriesDto.Description,
                AgeRating = seriesDto.AgeRating,
                ImageURL = seriesDto.ImageURL,
                Genre = seriesDto.Genre,
                ContentWarnings = seriesDto.ContentWarnings,
                AvailableQualities = seriesDto.AvailableQualities,
                Seasons = new List<Season>()
            };

            _db.Series.Add(series);
            await _db.SaveChangesAsync();

            return series;
        }

        public async Task<Series?> UpdateSeriesAsync(int id, SeriesRequestDto seriesDto)
        {
            var series = await _db.Series.FindAsync(id);
            if (series == null)
            {
                return null;
            }

            series.Title = seriesDto.Title;
            series.Description = seriesDto.Description;
            series.AgeRating = seriesDto.AgeRating;
            series.ImageURL = seriesDto.ImageURL;
            series.Genre = seriesDto.Genre;
            series.ContentWarnings = seriesDto.ContentWarnings;
            series.AvailableQualities = seriesDto.AvailableQualities;

            await _db.SaveChangesAsync();
            return series;
        }

        public async Task<bool> DeleteSeriesAsync(int id)
        {
            var series = await _db.Series.FindAsync(id);
            if (series == null)
            {
                return false;
            }

            _db.Series.Remove(series);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<PersonalizedMoviesResult> GetPersonalizedMoviesAsync(int profileId)
        {
            var result = new PersonalizedMoviesResult();

            var profile = await _db.Profiles
                .Include(p => p.Preference)
                .FirstOrDefaultAsync(p => p.ProfileId == profileId);

            if (profile == null)
            {
                result.ProfileExists = false;
                result.Movies = new List<Movie>();
                return result;
            }

            result.ProfileExists = true;

            IQueryable<Movie> query = _db.Movies;

            // Age filter
            if (profile.Preference != null && profile.Preference.MinimumAge > 0)
            {
                query = query.Where(m => m.AgeRating <= profile.Preference.MinimumAge);
            }

            // Preferred genres
            if (profile.Preference != null && profile.Preference.PreferredGenres.Any())
            {
                var preferredGenres = profile.Preference.PreferredGenres
                    .Select(g => g.ToLower())
                    .ToList();

                query = query.Where(m => preferredGenres.Contains(m.Genre.ToLower()));
            }

            // Content type filter (movies/series/both)
            if (profile.Preference != null && !string.IsNullOrWhiteSpace(profile.Preference.ContentType))
            {
                var contentType = profile.Preference.ContentType.ToLower();
                if (contentType == "series")
                {
                    result.Movies = new List<Movie>();
                    return result;
                }
            }

            // Content warnings filters
            if (profile.Preference != null && profile.Preference.ContentFilters.Any())
            {
                var blockedWarnings = profile.Preference.ContentFilters
                    .Select(w => w.ToLower())
                    .ToList();

                query = query.Where(m =>
                    !m.ContentWarnings.Any(w => blockedWarnings.Contains(w.ToLower())));
            }

            result.Movies = await query.ToListAsync();
            return result;
        }

        public async Task<PersonalizedSeriesResult> GetPersonalizedSeriesAsync(int profileId)
        {
            var result = new PersonalizedSeriesResult();

            var profile = await _db.Profiles
                .Include(p => p.Preference)
                .FirstOrDefaultAsync(p => p.ProfileId == profileId);

            if (profile == null)
            {
                result.ProfileExists = false;
                result.Series = new List<Series>();
                return result;
            }

            result.ProfileExists = true;

            IQueryable<Series> query = _db.Series
                .Include(s => s.Seasons)
                    .ThenInclude(season => season.Episodes);

            // Age filter
            if (profile.Preference != null && profile.Preference.MinimumAge > 0)
            {
                query = query.Where(s => s.AgeRating <= profile.Preference.MinimumAge);
            }

            // Preferred genres
            if (profile.Preference != null && profile.Preference.PreferredGenres.Any())
            {
                var preferredGenres = profile.Preference.PreferredGenres
                    .Select(g => g.ToLower())
                    .ToList();

                query = query.Where(s => preferredGenres.Contains(s.Genre.ToLower()));
            }

            // Content type filter
            if (profile.Preference != null && !string.IsNullOrWhiteSpace(profile.Preference.ContentType))
            {
                var contentType = profile.Preference.ContentType.ToLower();
                if (contentType == "movies")
                {
                    result.Series = new List<Series>();
                    return result;
                }
            }

            // Content warnings filters
            if (profile.Preference != null && profile.Preference.ContentFilters.Any())
            {
                var blockedWarnings = profile.Preference.ContentFilters
                    .Select(w => w.ToLower())
                    .ToList();

                query = query.Where(s =>
                    !s.ContentWarnings.Any(w => blockedWarnings.Contains(w.ToLower())));
            }

            result.Series = await query.ToListAsync();
            return result;
        }
    }
}
