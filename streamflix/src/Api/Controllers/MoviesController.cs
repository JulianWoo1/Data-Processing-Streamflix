using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public MoviesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
    {
        var movies = await _db.Movies
            .Include(m => m.Genre)
            .Include(m => m.ContentWarnings)
            .Include(m => m.AvailableQualities)
            .ToListAsync();

        var movieDtos = movies.Select(m => new MovieDto(
            m.ContentId,
            m.Title,
            m.Description,
            m.AgeRating,
            m.ImageURL,
            m.Duration,
            m.Genre.GenreType.ToString(),
            m.ContentWarnings.Select(cw => cw.ContentWarningType.ToString()).ToList(),
            m.AvailableQualities.Select(q => q.QualityType.ToString()).ToList()
        ));

        return Ok(movieDtos);
    }

    [HttpPost]
    public async Task<ActionResult<MovieDto>> CreateMovie(CreateMovieDto request)
    {
        var genre = await _db.Genres.FirstOrDefaultAsync(g => g.GenreType == request.Genre);
        if (genre == null) return BadRequest("Invalid Genre");

        var qualities = await _db.Qualities
            .Where(q => request.AvailableQualities.Contains(q.QualityType))
            .ToListAsync();

        var warnings = await _db.ContentWarnings
            .Where(w => request.ContentWarnings.Contains(w.ContentWarningType))
            .ToListAsync();

        var movie = new Movie
        {
            Title = request.Title,
            Description = request.Description,
            AgeRating = request.AgeRating,
            ImageURL = request.ImageURL,
            Duration = request.Duration,
            Genre = genre,
            AvailableQualities = qualities,
            ContentWarnings = warnings
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
            movie.Genre.GenreType.ToString(),
            movie.ContentWarnings.Select(cw => cw.ContentWarningType.ToString()).ToList(),
            movie.AvailableQualities.Select(q => q.QualityType.ToString()).ToList()
        );

        return CreatedAtAction(nameof(GetMovies), new { id = movie.ContentId }, responseDto);
    }
}
