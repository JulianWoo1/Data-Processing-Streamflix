using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WatchlistController : ControllerBase
{
    private readonly IWatchlistService _watchlistService;

    private readonly IProfileService _profileService;
    public WatchlistController(IWatchlistService watchlistService, IProfileService profileService)
    {
        _watchlistService = watchlistService;
        _profileService = profileService;
    }

    private int GetCurrentAccountId()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new InvalidOperationException("No account id claim present in token.");
        return int.Parse(claim.Value);
    }

    // Watchlist Endpoints\\
    [HttpGet("profile/{profileId}")]
    public async Task<ActionResult<WatchlistDto>> GetWatchlistByProfileId(int profileId)
    {
        var profile = await _profileService.GetProfileAsync(profileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();
        var watchlist = await _watchlistService.GetWatchlistByProfileIdAsync(profileId);

        if (watchlist == null) return NotFound();

        return Ok(ToWatchlistDto(watchlist));
    }

    [HttpPost("profile/{profileId}/add/{contentId}")]
    public async Task<ActionResult<WatchlistDto>> AddContentToWatchlist(int profileId, int contentId)
    {
        var profile = await _profileService.GetProfileAsync(profileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

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
        var profile = await _profileService.GetProfileAsync(profileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        var removed = await _watchlistService.RemoveFromWatchlistAsync(profileId, contentId);

        if (!removed) return NotFound();

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
