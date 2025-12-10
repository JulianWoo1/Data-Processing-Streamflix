using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    private int GetCurrentAccountId()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new InvalidOperationException("No account id claim present.");

        return int.Parse(claim.Value);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
    {
        var profiles = await _profileService.GetProfilesAsync();
        var currentAccountId = GetCurrentAccountId();

        return Ok(profiles.Where(p => p.AccountId == currentAccountId));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(int id)
    {
        var profile = await _profileService.GetProfileAsync(id);
        if (profile == null) return NotFound();

        var currentAccountId = GetCurrentAccountId();
        if (profile.AccountId != currentAccountId) return Forbid();

        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<ProfileDto>> CreateProfile(CreateProfileDto request)
    {
        var currentAccountId = GetCurrentAccountId();
        if (request.AccountId != currentAccountId) return Forbid();
        var created = await _profileService.CreateProfileAsync(request);

        return CreatedAtAction(nameof(GetProfile), new { id = created.ProfileId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, UpdateProfileDto request)
    {
        var existing = await _profileService.GetProfileAsync(id);
        if (existing == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (existing.AccountId != currentAccountId) return Forbid();
        var success = await _profileService.UpdateProfileAsync(id, request);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfile(int id)
    {
        var existing = await _profileService.GetProfileAsync(id);
        if (existing == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (existing.AccountId != currentAccountId) return Forbid();
        var success = await _profileService.DeleteProfileAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}/preference")]
    public async Task<IActionResult> UpdatePreference(int id, UpdateProfilePreferenceDto request)
    {
        var existing = await _profileService.GetProfileAsync(id);
        if (existing == null) return NotFound();
        var currentAccountId = GetCurrentAccountId();
        if (existing.AccountId != currentAccountId) return Forbid();
        var success = await _profileService.UpdatePreferenceAsync(id, request);
        if (!success) return NotFound();

        return NoContent();
    }

    private static ProfileDto ToDto(Profile p) =>
        new ProfileDto(
            p.ProfileId,
            p.AccountId,
            p.Name,
            p.AgeCategory,
            p.ImageUrl,
            p.Preference == null ? null : new ProfilePreferenceDto(
                p.Preference.PreferredGenres,
                p.Preference.ContentType,
                p.Preference.MinimumAge,
                p.Preference.ContentFilters
            ),
            p.ViewingHistories.Select(v => new ViewingHistoryDto(
                v.ViewingHistoryId,
                v.ProfileId,
                v.ContentId,
                v.EpisodeId,
                v.StartTime,
                v.EndTime,
                v.LastPosition,
                v.IsCompleted
            ))
        );
}
