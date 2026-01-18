using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamflix.Api.Services
{
    public interface IWatchlistService
    {
        Task<Watchlist?> GetWatchlistByProfileIdAsync(int profileId);
        Task<Watchlist> AddContentToWatchlistAsync(int profileId, int contentId);
        Task<bool> RemoveFromWatchlistAsync(int profileId, int contentId);
    }

    public class WatchlistService : IWatchlistService
    {
        private readonly ApplicationDbContext _db;

        public WatchlistService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Watchlist?> GetWatchlistByProfileIdAsync(int profileId)
        {
            return await _db.Watchlists
                .Include(w => w.Items)
                    .ThenInclude(wc => wc.Content)
                .FirstOrDefaultAsync(w => w.ProfileId == profileId);
        }

        public async Task<Watchlist> AddContentToWatchlistAsync(int profileId, int contentId)
        {
            // Ensure profile exists
            var profileExists = await _db.Profiles.AnyAsync(p => p.ProfileId == profileId);
            if (!profileExists)
            {
                throw new KeyNotFoundException("Profile not found.");
            }

            // Ensure content exists
            var contentExists = await _db.Contents.AnyAsync(c => c.ContentId == contentId);
            if (!contentExists)
            {
                throw new KeyNotFoundException("Content not found.");
            }

            // Load or create watchlist
            var watchlist = await _db.Watchlists
                .Include(w => w.Items)
                .FirstOrDefaultAsync(w => w.ProfileId == profileId);

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

            // Prevent duplicates
            if (watchlist.Items.Any(wc => wc.ContentId == contentId))
            {
                throw new InvalidOperationException("Content already exists in the watchlist.");
            }

            watchlist.Items.Add(new WatchlistContent
            {
                ContentId = contentId,
                DateAdded = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();

            // Reload with Content included for DTO mapping
            watchlist = await _db.Watchlists
                .Include(w => w.Items)
                    .ThenInclude(wc => wc.Content)
                .FirstAsync(w => w.WatchlistId == watchlist.WatchlistId);

            return watchlist;
        }

        public async Task<bool> RemoveFromWatchlistAsync(int profileId, int contentId)
        {
            var item = await _db.WatchlistContents
                .Include(wc => wc.Watchlist)
                .FirstOrDefaultAsync(wc =>
                    wc.Watchlist.ProfileId == profileId &&
                    wc.ContentId == contentId);

            if (item == null)
            {
                return false;
            }

            _db.WatchlistContents.Remove(item);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
