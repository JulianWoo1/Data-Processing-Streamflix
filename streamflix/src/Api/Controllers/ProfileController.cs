using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ProfileController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET all profiles
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProfileDto>>> GetProfiles()
    {
        var profiles = await _db.Profiles
            .Include(p => p.Preference)
            .ToListAsync();

        return Ok(profiles.Select(ToDto));
    }

    // GET profile by id
     [HttpGet("{id}")]
    public async Task<ActionResult<ProfileDto>> GetProfile(int id)
    {
        var profile = await _db.Profiles
            .Include(p => p.Preference)
            .FirstOrDefaultAsync(p => p.ProfileId == id);

        if (profile == null) return NotFound();
        return Ok(ToDto(profile));
    }

    // POST create profile
    [HttpPost]
    public async Task<ActionResult<ProfileDto>> CreateProfile(CreateProfileDto request)
    {
        var profile = new Profile
        {
            AccountId = request.AccountId,
            Name = request.Name,
            AgeCategory = request.AgeCategory,
            ImageUrl = request.ImageUrl
        };

        _db.Profiles.Add(profile);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProfile), new { id = profile.ProfileId }, ToDto(profile));
    }

    // PUT update profile
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProfile(int id, UpdateProfileDto request)
    {
        var profile = await _db.Profiles.FindAsync(id);
        if (profile == null) return NotFound();

        profile.Name = request.Name;
        profile.AgeCategory = request.AgeCategory;
        profile.ImageUrl = request.ImageUrl;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE profile
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProfile(int id)
    {
        var profile = await _db.Profiles.FindAsync(id);
        if (profile == null) return NotFound();

        _db.Profiles.Remove(profile);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // PUT update profile preference
    [HttpPut("{id}/preference")]
    public async Task<IActionResult> UpdatePreference(int id, UpdateProfilePreferenceDto request)
    {
        var profile = await _db.Profiles
            .Include(p => p.Preference)
            .FirstOrDefaultAsync(p => p.ProfileId == id);

        if (profile == null) return NotFound();

        if (profile.Preference == null)
        {
            profile.Preference = new ProfilePreference { ProfileId = id };
            _db.ProfilePreference.Add(profile.Preference);
        }

        profile.Preference.PreferredGenres = request.PreferredGenres;
        profile.Preference.ContentType = request.ContentType;
        profile.Preference.MinimumAge = request.MinimumAge;
        profile.Preference.ContentFilters = request.ContentFilters;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // Helper method: map entity → DTO
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
            )
        );
}
