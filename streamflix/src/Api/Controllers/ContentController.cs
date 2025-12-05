using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ContentController(ApplicationDbContext db)
    {
        _db = db;
    }

    //Movie Endpoints\\
    // Get all movies
    [HttpGet("movies")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
    {
        var movies = await _db.Movies
            .ToListAsync();

        return Ok(movies.Select(ToMovieDto));
    }

    // Get movie by ID
    [HttpGet("movies/{id}")]
    public async Task<ActionResult<MovieDto>> GetMovie(int id)
    {
        var movie = await _db.Movies
            .FirstOrDefaultAsync(m => m.ContentId == id);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(ToMovieDto(movie));
    }

    // Get movie by Title
    [HttpGet("movies/title/{title}")]
    public async Task<ActionResult<MovieDto>> GetMovieByTitle(string title)
    {
        var normalizedTitle = title.ToLower();
        var movie = await _db.Movies
            .FirstOrDefaultAsync(m => m.Title.ToLower() == normalizedTitle);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(ToMovieDto(movie));
    }

    // Get movies by Genre
    [HttpGet("movies/genre/{genre}")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByGenre(string genre)
    {
        var normalizedGenre = genre.ToLower();
        var movies = await _db.Movies
            .Where(m => m.Genre.ToLower() == normalizedGenre)
            .ToListAsync();

        return Ok(movies.Select(ToMovieDto));
    }

    // Series Endpoints\\
    // Get all series
    [HttpGet("series")]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetSeries()
    {
        var series = await _db.Series
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .ToListAsync();

        return Ok(series.Select(ToSeriesWithSeasonsDto));
    }

    // Get series by ID
    [HttpGet("series/{id}")]
    public async Task<ActionResult<SeriesDto>> GetSeries(int id)
    {
        var series = await _db.Series
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .FirstOrDefaultAsync(s => s.ContentId == id);

        if (series == null)
        {
            return NotFound();
        }

        return Ok(ToSeriesWithSeasonsDto(series));
    }

    // Get series by Title
    [HttpGet("series/title/{title}")]
    public async Task<ActionResult<SeriesDto>> GetSeriesByTitle(string title)
    {
        var normalizedTitle = title.ToLower();
        var series = await _db.Series
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .FirstOrDefaultAsync(s => s.Title.ToLower() == normalizedTitle);

        if (series == null)
        {
            return NotFound();
        }

        return Ok(ToSeriesWithSeasonsDto(series));
    }

    // Get series by Genre
    [HttpGet("series/genre/{genre}")]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetSeriesByGenre(string genre)
    {
        var normalizedGenre = genre.ToLower();
        var seriesList = await _db.Series
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .Where(s => s.Genre.ToLower() == normalizedGenre)
            .ToListAsync();

        return Ok(seriesList.Select(ToSeriesWithSeasonsDto));
    }

    // TODO: Potentially switch to AutoMapper
    //Helper Methods\\
    private static MovieDto ToMovieDto(Movie m) =>
        new MovieDto(
            m.ContentId,
            m.Title,
            m.Description,
            m.AgeRating,
            m.ImageURL,
            m.Duration,
            m.Genre,
            m.ContentWarnings,
            m.AvailableQualities
        );

    // Series helpers
    // Summary (no seasons/episodes)
    private static SeriesDto ToSeriesSummaryDto(Series s) =>
        new SeriesDto(
            s.ContentId,
            s.Title,
            s.Description,
            s.AgeRating,
            s.ImageURL,
            s.TotalSeasons,
            s.Genre,
            s.ContentWarnings,
            s.AvailableQualities,
            new List<SeasonDto>()
        );

    // Single episode
    private static EpisodeDto ToEpisodeDto(Episode e) =>
        new EpisodeDto(
            e.EpisodeId,
            e.EpisodeNumber,
            e.Title,
            e.Duration
        );

    // Single season with episodes (handles null Episodes)
    private static SeasonDto ToSeasonDto(Season season) =>
        new SeasonDto(
            season.SeasonId,
            season.SeasonNumber,
            season.TotalEpisodes,
            (season.Episodes ?? new List<Episode>())
                .Select(ToEpisodeDto)
                .ToList()
        );

    // Full series with seasons + episodes (handles null Seasons)
    private static SeriesDto ToSeriesWithSeasonsDto(Series s) =>
        new SeriesDto(
            s.ContentId,
            s.Title,
            s.Description,
            s.AgeRating,
            s.ImageURL,
            s.TotalSeasons,
            s.Genre,
            s.ContentWarnings,
            s.AvailableQualities,
            (s.Seasons ?? new List<Season>())
                .Select(ToSeasonDto)
                .ToList()
        );


}
