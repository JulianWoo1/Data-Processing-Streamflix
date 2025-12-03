using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WatchlistController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public WatchlistController(ApplicationDbContext db)
    {
        _db = db;
    }

    // Watchlist Endpoints\\

    // Get watchlist by Profile ID
    [HttpGet("profile/{profileId}")]
    public async Task<ActionResult<WatchlistDto>> GetWatchlistByProfileId(int profileId)
    {
        var watchlist = await _db.Watchlists
            .Include(w => w.Items)
                .ThenInclude(wc => wc.Content)
            .FirstOrDefaultAsync(w => w.ProfileId == profileId);

        if (watchlist == null)
        {
            return NotFound();
        }

        return Ok(ToWatchlistDto(watchlist));
    }

    // Add content to watchlist (auto-creates watchlist if needed)
    [HttpPost("profile/{profileId}/add/{contentId}")]
    public async Task<ActionResult<WatchlistDto>> AddContentToWatchlist(int profileId, int contentId)
    {
        // Optional but safer: ensure profile exists
        var profileExists = await _db.Profiles.AnyAsync(p => p.ProfileId == profileId);
        if (!profileExists)
        {
            return NotFound("Profile not found.");
        }

        // Optional: ensure content exists
        var contentExists = await _db.Contents.AnyAsync(c => c.ContentId == contentId);
        if (!contentExists)
        {
            return NotFound("Content not found.");
        }

        var watchlist = await _db.Watchlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.ProfileId == profileId);

        // Auto-create watchlist if it doesn't exist yet
        if (watchlist == null)
        {
            watchlist = new Watchlist
            {
                ProfileId = profileId,
                CreatedAt = DateTime.UtcNow,
                Items = new List<WatchlistContent>()
            };
            _db.Watchlists.Add(watchlist);
        }

        if (watchlist.Items.Any(wc => wc.ContentId == contentId))
        {
            return BadRequest("Content already exists in the watchlist.");
        }

        watchlist.Items.Add(new WatchlistContent
        {
            ContentId = contentId,
            DateAdded = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();

        // Reload including Content so DTO has full info
        watchlist = await _db.Watchlists
            .Include(w => w.Items)
                .ThenInclude(wc => wc.Content)
            .FirstAsync(w => w.WatchlistId == watchlist.WatchlistId);

        return Ok(ToWatchlistDto(watchlist));
    }

    // Remove content from watchlist
    [HttpDelete("profile/{profileId}/remove/{contentId}")]
    public async Task<ActionResult> RemoveFromWatchListByContent(int profileId, int contentId)
    {
        var item = await _db.WatchlistContents
            .Include(wc => wc.Watchlist)
            .FirstOrDefaultAsync(wc =>
                wc.Watchlist.ProfileId == profileId &&
                wc.ContentId == contentId);

        if (item == null)
        {
            return NotFound();
        }

        _db.WatchlistContents.Remove(item);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    // Helper Methods\\

    // Convert Watchlist entity to WatchlistDto
    private static WatchlistDto ToWatchlistDto(Watchlist watchlist) =>
        new WatchlistDto(
            watchlist.WatchlistId,
            watchlist.ProfileId,
            watchlist.CreatedAt,
            watchlist.Items.Select(wc => new WatchlistContentDto(
                wc.WatchlistContentId,
                wc.ContentId,
                wc.Content.Title,
                wc.Content.Description,
                wc.Content.AgeRating,
                wc.Content.ImageURL,
                wc.Content.Genre,
                wc.Content.ContentWarnings,
                wc.Content.AvailableQualities,
                wc.DateAdded
            )).ToList()
        );
}
