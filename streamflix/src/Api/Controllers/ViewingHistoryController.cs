using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViewingHistoryController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ViewingHistoryController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET all viewing history for a profile
    [HttpGet("{profileId}")]
    public async Task<ActionResult<IEnumerable<ViewingHistoryDto>>> GetHistory(int profileId)
    {
        var histories = await _db.ViewingHistories
            .Where(v => v.ProfileId == profileId)
            .ToListAsync();

        return Ok(histories.Select(ToDto));
    }

    // POST start viewing content
    [HttpPost]
    public async Task<ActionResult<ViewingHistoryDto>> StartViewing(ViewingHistoryDto request)
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

        return CreatedAtAction(nameof(GetHistory), new { profileId = newHistory.ProfileId }, ToDto(newHistory));
    }

    // PUT update progress for a viewing history entry
    [HttpPut("{viewingHistoryId}")]
    public async Task<IActionResult> UpdateProgress(int viewingHistoryId, UpdateViewingHistoryDto request)
    {
        var history = await _db.ViewingHistories.FindAsync(viewingHistoryId);
        if (history == null) return NotFound();

        history.LastPosition = request.LastPosition;
        history.IsCompleted = request.IsCompleted;

        if (request.IsCompleted && history.EndTime == null)
        {
            history.EndTime = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // POST mark viewing as completed
    [HttpPost("{viewingHistoryId}/complete")]
    public async Task<IActionResult> MarkAsCompleted(int viewingHistoryId)
    {
        var history = await _db.ViewingHistories.FindAsync(viewingHistoryId);
        if (history == null) return NotFound();

        history.IsCompleted = true;
        history.EndTime = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // GET resume content (fetch last position to continue watching)
    [HttpGet("resume/{profileId}/{contentId}")]
    public async Task<ActionResult<ViewingHistoryDto?>> ResumeContent(int profileId, int contentId)
    {
        var history = await _db.ViewingHistories
            .Where(v => v.ProfileId == profileId && v.ContentId == contentId && !v.IsCompleted)
            .OrderByDescending(v => v.StartTime)
            .FirstOrDefaultAsync();

        if (history == null) return NotFound();

        return Ok(ToDto(history));
    }

    // Helper method: map entity → DTO
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
