using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json", "application/xml", "text/csv")]
public class ViewingHistoryController : ControllerBase
{
    private readonly IViewingHistoryService _service;
    private readonly IProfileService _profileService;

    public ViewingHistoryController(IViewingHistoryService service, IProfileService profileService)
    {
        _service = service;
        _profileService = profileService;
    }

    private int GetCurrentAccountId()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new InvalidOperationException("Account id claim not found.");
        return int.Parse(claim.Value);
    }

    // GET all viewing history for a profile
    [HttpGet("{profileId}")]
    public async Task<ActionResult<ViewingHistoriesDto>> GetHistory(int profileId)
    {
        var profile = await _profileService.GetProfileAsync(profileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        var histories = await _service.GetHistoryAsync(profileId);
        return Ok(new ViewingHistoriesDto { ViewingHistories = histories.ToList() });
    }

    // POST start viewing content
    [HttpPost]
    public async Task<ActionResult<ViewingHistoryDto>> StartViewing(CreateViewingHistoryDto request)
    {
        var profile = await _profileService.GetProfileAsync(request.ProfileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        var newHistory = await _service.StartViewingAsync(request);
        return CreatedAtAction(nameof(GetHistory), new { profileId = newHistory.ProfileId }, newHistory);
    }

    // PUT update progress for a viewing history entry
    [HttpPut("{viewingHistoryId}")]
    public async Task<IActionResult> UpdateProgress(int viewingHistoryId, UpdateViewingHistoryDto request)
    {
        var history = await _service.GetHistoryByIdAsync(viewingHistoryId);
        if (history == null) return NotFound();
        var profile = await _profileService.GetProfileAsync(history.ProfileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        var updated = await _service.UpdateProgressAsync(viewingHistoryId, request);
        if (!updated) return NotFound();

        return NoContent();
    }

    // POST mark viewing as completed
    [HttpPost("{viewingHistoryId}/complete")]
    public async Task<IActionResult> MarkAsCompleted(int viewingHistoryId)
    {
        var history = await _service.GetHistoryByIdAsync(viewingHistoryId);
        if (history == null) return NotFound();
        var profile = await _profileService.GetProfileAsync(history.ProfileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        var updated = await _service.MarkAsCompletedAsync(viewingHistoryId);
        if (!updated) return NotFound();

        return NoContent();
    }

    // GET resume content (fetch last position to continue watching)
    [HttpGet("resume/{profileId}/{contentId}")]
    public async Task<ActionResult<ViewingHistoryDto?>> ResumeContent(int profileId, int contentId)
    {
        var profile = await _profileService.GetProfileAsync(profileId);
        if (profile == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        var history = await _service.ResumeContentAsync(profileId, contentId);
        if (history == null) return NotFound();

        return Ok(history);
    }
}
