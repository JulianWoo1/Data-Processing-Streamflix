using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;

    public ContentController(IContentService contentService)
    {
        _contentService = contentService;
    }

    [HttpGet("movies")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
    {
        var movies = await _contentService.GetMoviesAsync();

        return Ok(movies.Select(ToMovieDto));
    }

    [HttpGet("movies/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<MovieDto>> GetMovie(int id)
    {
        var movie = await _contentService.GetMovieByIdAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(ToMovieDto(movie));
    }

    [HttpGet("movies/personalized")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetPersonalizedMovies([FromQuery] int profileId)
    {

        var result = await _contentService.GetPersonalizedMoviesAsync(profileId);

        if (!result.ProfileExists)
        {
            return NotFound("Profile not found.");
        }

        return Ok(result.Movies.Select(ToMovieDto));
    }

    [HttpGet("movies/title/{title}")]
    [AllowAnonymous]
    public async Task<ActionResult<MovieDto>> GetMovieByTitle(string title)
    {
        var movie = await _contentService.GetMovieByTitleAsync(title);

        if (movie == null)
        {
            return NotFound();
        }

        return Ok(ToMovieDto(movie));
    }

    [HttpGet("movies/genre/{genre}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByGenre(string genre)
    {
        var movies = await _contentService.GetMoviesByGenreAsync(genre);

        return Ok(movies.Select(ToMovieDto));
    }

    [HttpGet("series")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetSeries()
    {
        var series = await _contentService.GetSeriesAsync();

        return Ok(series.Select(ToSeriesWithSeasonsDto));
    }

    [HttpGet("series/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<SeriesDto>> GetSeries(int id)
    {
        var series = await _contentService.GetSeriesByIdAsync(id);

        if (series == null)
        {
            return NotFound();
        }

        return Ok(ToSeriesWithSeasonsDto(series));
    }

    [HttpGet("series/title/{title}")]
    [AllowAnonymous]
    public async Task<ActionResult<SeriesDto>> GetSeriesByTitle(string title)
    {
        var series = await _contentService.GetSeriesByTitleAsync(title);

        if (series == null)
        {
            return NotFound();
        }

        return Ok(ToSeriesWithSeasonsDto(series));
    }

    [HttpGet("series/personalized")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetPersonalizedSeries([FromQuery] int profileId)
    {
        var result = await _contentService.GetPersonalizedSeriesAsync(profileId);

        if (!result.ProfileExists)
        {
            return NotFound("Profile not found.");
        }

        return Ok(result.Series.Select(ToSeriesWithSeasonsDto));
    }

    [HttpGet("series/genre/{genre}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetSeriesByGenre(string genre)
    {
        var seriesList = await _contentService.GetSeriesByGenreAsync(genre);

        return Ok(seriesList.Select(ToSeriesWithSeasonsDto));
    }

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
