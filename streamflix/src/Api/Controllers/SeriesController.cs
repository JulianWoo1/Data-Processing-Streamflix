using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SeriesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public SeriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SeriesDto>>> GetSeries()
    {
        var series = await _db.Series
            .Include(s => s.Genre)
            .Include(s => s.ContentWarnings)
            .Include(s => s.AvailableQualities)
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
            s.Genre.GenreType.ToString(),
            s.ContentWarnings.Select(cw => cw.ContentWarningType.ToString()).ToList(),
            s.AvailableQualities.Select(q => q.QualityType.ToString()).ToList(),
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

    [HttpPost]
    public async Task<ActionResult<SeriesDto>> CreateSeries(CreateSeriesDto request)
    {
        var genre = await _db.Genres.FirstOrDefaultAsync(g => g.GenreType == request.Genre);
        if (genre == null) return BadRequest("Invalid Genre");

        var qualities = await _db.Qualities
            .Where(q => request.AvailableQualities.Contains(q.QualityType))
            .ToListAsync();

        var warnings = await _db.ContentWarnings
            .Where(w => request.ContentWarnings.Contains(w.ContentWarningType))
            .ToListAsync();

        var series = new Series
        {
            Title = request.Title,
            Description = request.Description,
            AgeRating = request.AgeRating,
            ImageURL = request.ImageURL,
            TotalSeasons = request.TotalSeasons,
            Genre = genre,
            AvailableQualities = qualities,
            ContentWarnings = warnings
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
            series.Genre.GenreType.ToString(),
            series.ContentWarnings.Select(cw => cw.ContentWarningType.ToString()).ToList(),
            series.AvailableQualities.Select(q => q.QualityType.ToString()).ToList(),
            new List<SeasonDto>()
        );

        return CreatedAtAction(nameof(GetSeries), new { id = series.ContentId }, responseDto);
    }
}
