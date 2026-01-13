using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Services
{
    public interface IProfileService
    {
        Task<IEnumerable<ProfileDto>> GetProfilesAsync();
        Task<ProfileDto?> GetProfileAsync(int id);
        Task<ProfileDto> CreateProfileAsync(CreateProfileDto request);
        Task<bool> UpdateProfileAsync(int id, UpdateProfileDto request);
        Task<bool> DeleteProfileAsync(int id);
        Task<bool> UpdatePreferenceAsync(int id, UpdateProfilePreferenceDto request);
    }

    public class ProfileService : IProfileService
    {
        private readonly ApplicationDbContext _db;

        public ProfileService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ProfileDto>> GetProfilesAsync()
        {
            var profiles = await _db.Profiles
                .Include(p => p.Preference)
                .Include(p => p.ViewingHistories)
                .ToListAsync();

            return profiles.Select(ToDto).ToList();
        }

        public async Task<ProfileDto?> GetProfileAsync(int id)
        {
            var profile = await _db.Profiles
                .Include(p => p.Preference)
                .Include(p => p.ViewingHistories)
                .FirstOrDefaultAsync(p => p.ProfileId == id);

            if (profile == null)
            {
                return null;
            }

            return ToDto(profile);
        }

        public async Task<ProfileDto> CreateProfileAsync(CreateProfileDto request)
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

            profile = await _db.Profiles
                .Include(p => p.Preference)
                .Include(p => p.ViewingHistories)
                .FirstAsync(p => p.ProfileId == profile.ProfileId);

            return ToDto(profile);
        }

        public async Task<bool> UpdateProfileAsync(int id, UpdateProfileDto request)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile == null)
            {
                return false;
            }

            profile.Name = request.Name;
            profile.AgeCategory = request.AgeCategory;
            profile.ImageUrl = request.ImageUrl;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProfileAsync(int id)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile == null)
            {
                return false;
            }

            _db.Profiles.Remove(profile);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePreferenceAsync(int id, UpdateProfilePreferenceDto request)
        {
            var profile = await _db.Profiles
                .Include(p => p.Preference)
                .FirstOrDefaultAsync(p => p.ProfileId == id);

            if (profile == null)
            {
                return false;
            }

            if (profile.Preference == null)
            {
                profile.Preference = new ProfilePreference { ProfileId = id };
                _db.ProfilePreferences.Add(profile.Preference);
            }

            profile.Preference.PreferredGenres = request.PreferredGenres;
            profile.Preference.ContentType = request.ContentType;
            profile.Preference.MinimumAge = request.MinimumAge;
            profile.Preference.ContentFilters = request.ContentFilters;

            await _db.SaveChangesAsync();
            return true;
        }

        private static ProfileDto ToDto(Profile p) =>
            new()
            {
                ProfileId = p.ProfileId,
                AccountId = p.AccountId,
                Name = p.Name,
                AgeCategory = p.AgeCategory,
                ImageUrl = p.ImageUrl,
                Preference = p.Preference == null
                    ? null
                    : new ProfilePreferenceDto
                    {
                        PreferredGenres = p.Preference.PreferredGenres,
                        ContentType = p.Preference.ContentType,
                        MinimumAge = p.Preference.MinimumAge,
                        ContentFilters = p.Preference.ContentFilters
                    },
                ViewingHistories = p.ViewingHistories.Select(v => new ViewingHistoryDto
                {
                    ViewingHistoryId = v.ViewingHistoryId,
                    ProfileId = v.ProfileId,
                    ContentId = v.ContentId,
                    EpisodeId = v.EpisodeId,
                    StartTime = v.StartTime,
                    EndTime = v.EndTime,
                    LastPosition = v.LastPosition,
                    IsCompleted = v.IsCompleted
                })
            };
    }
}
