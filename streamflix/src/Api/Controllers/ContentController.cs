using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json", "application/xml", "text/csv")]
public class ContentController : ControllerBase
{
    private readonly IContentService _contentService;

    private readonly IProfileService _profileService;

    public ContentController(IContentService contentService, IProfileService profileService)

    {
        _contentService = contentService;
        _profileService = profileService;
    }

    private int GetCurrentAccountId()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new InvalidOperationException("No account id claim present.");
        return int.Parse(claim.Value);
    }

    [HttpGet("movies")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMovies([FromQuery] string? title, [FromQuery] string? genre)
    {
        if (!string.IsNullOrEmpty(title))
        {
            var movie = await _contentService.GetMovieByTitleAsync(title);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(ToMovieDto(movie));
        }

        if (!string.IsNullOrEmpty(genre))
        {
            var movies = await _contentService.GetMoviesByGenreAsync(genre);
            return Ok(new MoviesDto { Movies = movies.Select(ToMovieDto).ToList() });
        }

        var allMovies = await _contentService.GetMoviesAsync();
        return Ok(new MoviesDto { Movies = allMovies.Select(ToMovieDto).ToList() });
    }

    [HttpGet("movies/{id:int}")]
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
    public async Task<IActionResult> GetPersonalizedMovies([FromQuery] int profileId)
    {
        var validationResult = await ValidateProfileOwnership(profileId);
        if (validationResult != null)
        {
            return validationResult;
        }

        var result = await _contentService.GetPersonalizedMoviesAsync(profileId);

        return Ok(result.Movies.Select(ToMovieDto));
    }

    [HttpPost("movies")]
    [Authorize]
    public async Task<ActionResult<MovieDto>> CreateMovie([FromBody] MovieRequestDto movieDto)
    {
        var movie = await _contentService.CreateMovieAsync(movieDto);
        return CreatedAtAction(nameof(GetMovie), new { id = movie.ContentId }, ToMovieDto(movie));
    }

    [HttpPut("movies/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieRequestDto movieDto)
    {
        var movie = await _contentService.UpdateMovieAsync(id, movieDto);
        if (movie == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("movies/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        var success = await _contentService.DeleteMovieAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpGet("series")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSeries([FromQuery] string? title, [FromQuery] string? genre)
    {
        if (!string.IsNullOrEmpty(title))
        {
            var series = await _contentService.GetSeriesByTitleAsync(title);
            if (series == null)
            {
                return NotFound();
            }
            return Ok(ToSeriesWithSeasonsDto(series));
        }
        if (!string.IsNullOrEmpty(genre))
        {
            var seriesList = await _contentService.GetSeriesByGenreAsync(genre);
            return Ok(new SeriesListDto { Series = seriesList.Select(ToSeriesWithSeasonsDto).ToList() });
        }

        var allSeries = await _contentService.GetSeriesAsync();
        return Ok(new SeriesListDto { Series = allSeries.Select(ToSeriesWithSeasonsDto).ToList() });
    }

    [HttpGet("series/{id:int}")]
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

    [HttpGet("series/personalized")]
    [Authorize]
    public async Task<IActionResult> GetPersonalizedSeries([FromQuery] int profileId)
    {
        var validationResult = await ValidateProfileOwnership(profileId);
        if (validationResult != null)
        {
            return validationResult;
        }

        var result = await _contentService.GetPersonalizedSeriesAsync(profileId);

        return Ok(result.Series.Select(ToSeriesWithSeasonsDto));
    }

    [HttpPost("series")]
    [Authorize]
    public async Task<ActionResult<SeriesDto>> CreateSeries([FromBody] SeriesRequestDto seriesDto)
    {
        var series = await _contentService.CreateSeriesAsync(seriesDto);
        return CreatedAtAction(nameof(GetSeries), new { id = series.ContentId }, ToSeriesWithSeasonsDto(series));
    }

    [HttpPut("series/{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateSeries(int id, [FromBody] SeriesRequestDto seriesDto)
    {
        var series = await _contentService.UpdateSeriesAsync(id, seriesDto);
        if (series == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("series/{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteSeries(int id)
    {
        var success = await _contentService.DeleteSeriesAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    private async Task<IActionResult?> ValidateProfileOwnership(int profileId)
    {
        var profile = await _profileService.GetProfileAsync(profileId);
        if (profile == null)
        {
            return NotFound("Profile not found.");
        }

        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId)
        {
            return Forbid();
        }

        return null;
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
