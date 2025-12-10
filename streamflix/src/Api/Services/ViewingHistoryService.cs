using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Streamflix.Api.Services
{
    public interface IViewingHistoryService
    {
        Task<IEnumerable<ViewingHistoryDto>> GetHistoryAsync(int profileId);
        Task<ViewingHistoryDto> StartViewingAsync(ViewingHistoryDto request);
        Task<bool> UpdateProgressAsync(int viewingHistoryId, UpdateViewingHistoryDto request);
        Task<bool> MarkAsCompletedAsync(int viewingHistoryId);
        Task<ViewingHistoryDto?> ResumeContentAsync(int profileId, int contentId);
    }

    public class ViewingHistoryService : IViewingHistoryService
    {
        private readonly ApplicationDbContext _db;

        public ViewingHistoryService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ViewingHistoryDto>> GetHistoryAsync(int profileId)
        {
            var histories = await _db.ViewingHistories
                .Where(v => v.ProfileId == profileId)
                .ToListAsync();

            return histories.Select(ToDto).ToList();
        }

        public async Task<ViewingHistoryDto> StartViewingAsync(ViewingHistoryDto request)
        {
            var newHistory = new ViewingHistory
            {
                ProfileId = request.ProfileId,
                ContentId = request.ContentId,
                EpisodeId = request.EpisodeId,
                StartTime = DateTime.UtcNow,
                LastPosition = 0,
                IsCompleted = false
            };

            _db.ViewingHistories.Add(newHistory);
            await _db.SaveChangesAsync();

            return ToDto(newHistory);
        }

        public async Task<bool> UpdateProgressAsync(int viewingHistoryId, UpdateViewingHistoryDto request)
        {
            var history = await _db.ViewingHistories.FindAsync(viewingHistoryId);
            if (history == null)
            {
                return false;
            }

            history.LastPosition = request.LastPosition;
            history.IsCompleted = request.IsCompleted;

            if (request.IsCompleted && history.EndTime == null)
            {
                history.EndTime = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsCompletedAsync(int viewingHistoryId)
        {
            var history = await _db.ViewingHistories.FindAsync(viewingHistoryId);
            if (history == null)
            {
                return false;
            }

            history.IsCompleted = true;
            history.EndTime = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<ViewingHistoryDto?> ResumeContentAsync(int profileId, int contentId)
        {
            var history = await _db.ViewingHistories
                .Where(v => v.ProfileId == profileId && v.ContentId == contentId && !v.IsCompleted)
                .OrderByDescending(v => v.StartTime)
                .FirstOrDefaultAsync();

            if (history == null)
            {
                return null;
            }

            return ToDto(history);
        }

        private static ViewingHistoryDto ToDto(ViewingHistory v) =>
            new ViewingHistoryDto(
                v.ViewingHistoryId,
                v.ProfileId,
                v.ContentId,
                v.EpisodeId,
                v.StartTime,
                v.EndTime,
                v.LastPosition,
                v.IsCompleted
            );
    }
}
