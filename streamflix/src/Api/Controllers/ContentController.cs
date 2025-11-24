using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ContentController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ContentController(ApplicationDbContext db)
    {
        _db = db;
    }

    //Movie Endpoints
    [HttpGet("movies")]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
    {
        var movies = await _db.Movies
            .ToListAsync();

        var movieDtos = movies.Select(m => new MovieDto(
            m.ContentId,
            m.Title,
            m.Description,
            m.AgeRating,
            m.ImageURL,
            m.Duration,
            m.Genre,
            m.ContentWarnings,
            m.AvailableQualities
        ));

        return Ok(movieDtos);
    }

    [HttpPost("movies")]
    public async Task<ActionResult<MovieDto>> CreateMovie(CreateMovieDto request)
    {
        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            AgeRating = request.AgeRating,
            ImageURL = request.ImageURL,
            Duration = request.Duration,
            Genre = request.Genre,
            AvailableQualities = request.AvailableQualities,
            ContentWarnings = request.ContentWarnings
        };

        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();

        var responseDto = new MovieDto(
            movie.ContentId,
            movie.Title,
            movie.Description,
            movie.AgeRating,
            movie.ImageURL,
            movie.Duration,
            movie.Genre,
            movie.ContentWarnings,
            movie.AvailableQualities
        );

        return CreatedAtAction(nameof(GetMovies), new { id = movie.ContentId }, responseDto);
    }

    //Series Endpoints
    [HttpGet("series")]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetSeries()
    {
        var series = await _db.Series
            .Include(s => s.Seasons)
                .ThenInclude(season => season.Episodes)
            .ToListAsync();

        var seriesDtos = series.Select(s => new SeriesDto(
            s.ContentId,
            s.Title,
            s.Description,
            s.AgeRating,
            s.ImageURL,
            s.TotalSeasons,
            s.Genre,
            s.ContentWarnings,
            s.AvailableQualities,
            s.Seasons.Select(season => new SeasonDto(
                season.SeasonId,
                season.SeasonNumber,
                season.TotalEpisodes,
                season.Episodes.Select(e => new EpisodeDto(
                    e.EpisodeId,
                    e.EpisodeNumber,
                    e.Title,
                    e.Duration
                )).ToList()
            )).ToList()
        ));

        return Ok(seriesDtos);
    }

    [HttpPost("series")]
    public async Task<ActionResult<SeriesDto>> CreateSeries(CreateSeriesDto request)
    {
        var series = new Series
        {
            Title = request.Title,
            Description = request.Description,
            AgeRating = request.AgeRating,
            ImageURL = request.ImageURL,
            TotalSeasons = request.TotalSeasons,
            Genre = request.Genre,
            AvailableQualities = request.AvailableQualities,
            ContentWarnings = request.ContentWarnings
        };

        _db.Series.Add(series);
        await _db.SaveChangesAsync();

        var responseDto = new SeriesDto(
            series.ContentId,
            series.Title,
            series.Description,
            series.AgeRating,
            series.ImageURL,
            series.TotalSeasons,
            series.Genre,
            series.ContentWarnings,
            series.AvailableQualities,
            new List<SeasonDto>()
        );

        return CreatedAtAction(nameof(GetSeries), new { id = series.ContentId }, responseDto);
    }
}