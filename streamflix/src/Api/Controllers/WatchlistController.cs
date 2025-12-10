using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;

    public WatchlistController(IWatchlistService watchlistService)
    {
        _watchlistService = watchlistService;
    }

    // Watchlist Endpoints\\
    [HttpGet("profile/{profileId}")]
    public async Task<ActionResult<WatchlistDto>> GetWatchlistByProfileId(int profileId)
    {
        var watchlist = await _watchlistService.GetWatchlistByProfileIdAsync(profileId);

        if (watchlist == null)
        {
            return NotFound();
        }

        return Ok(ToWatchlistDto(watchlist));
    }

    [HttpPost("profile/{profileId}/add/{contentId}")]
    public async Task<ActionResult<WatchlistDto>> AddContentToWatchlist(int profileId, int contentId)
    {
        try
        {
            var watchlist = await _watchlistService.AddContentToWatchlistAsync(profileId, contentId);
            return Ok(ToWatchlistDto(watchlist));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("profile/{profileId}/remove/{contentId}")]
    public async Task<ActionResult> RemoveFromWatchListByContent(int profileId, int contentId)
    {
        var removed = await _watchlistService.RemoveFromWatchlistAsync(profileId, contentId);
        if (!removed)
        {
            return NotFound();
        }

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
