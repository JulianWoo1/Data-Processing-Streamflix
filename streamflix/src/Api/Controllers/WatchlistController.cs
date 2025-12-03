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

    //Watchlist Endpoints\\
    // Get watchlist by Profile ID
    [HttpGet("profile/{profileId}")]
    public async Task<ActionResult<IEnumerable<WatchlistDto>>> GetWatchlistByProfileId(int profileId)
    {
        var watchlist = _db.Watchlists
            .Include(w => w.Items)
            .ThenInclude(wc => wc.Content)
            .FirstOrDefault(w => w.ProfileId == profileId);

        if (watchlist == null)
        {
            return NotFound();
        }

        return Ok(ToWatchlistDto(watchlist));
    }

    // Add content to watchlist
    [HttpPost("profile/{profileId}/add/{contentId}")]
    public async Task<ActionResult> AddContentToWatchlist(int profileId, int contentId)
    {
        var watchlist = _db.Watchlists
            .Include(w => w.Items)
            .FirstOrDefault(w => w.ProfileId == profileId);

        if (watchlist == null)
        {
            return NotFound("Watchlist not found for the given profile ID.");
        }

        if (watchlist.Items.Any(wc => wc.ContentId == contentId))
        {
            return BadRequest("Content already exists in the watchlist.");
        }

        var newWatchlistContent = new WatchlistContent
        {
            WatchlistId = watchlist.WatchlistId,
            ContentId = contentId,
            DateAdded = DateTime.UtcNow
        };

        watchlist.Items.Add(newWatchlistContent);
        await _db.SaveChangesAsync();

        return Ok("Content added to watchlist.");
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


    //Helper Methods\\
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
