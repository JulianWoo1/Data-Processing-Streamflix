using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
    {
        var profiles = await _profileService.GetProfilesAsync();

        return Ok(profiles);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(int id)
    {
        var profile = await _profileService.GetProfileAsync(id);
        if (profile == null) return NotFound();

        return Ok(profile);
    }

    [HttpPost]
    public async Task<ActionResult<ProfileDto>> CreateProfile(CreateProfileDto request)
    {
        var created = await _profileService.CreateProfileAsync(request);

        return CreatedAtAction(nameof(GetProfile), new { id = created.ProfileId }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, UpdateProfileDto request)
    {
        var success = await _profileService.UpdateProfileAsync(id, request);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfile(int id)
    {
        var success = await _profileService.DeleteProfileAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpPut("{id}/preference")]
    public async Task<IActionResult> UpdatePreference(int id, UpdateProfilePreferenceDto request)
    {
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
