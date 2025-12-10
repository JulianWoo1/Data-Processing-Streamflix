using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.Services;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ViewingHistoryController : ControllerBase
{
    private readonly IViewingHistoryService _service;

    public ViewingHistoryController(IViewingHistoryService service)
    {
        _service = service;
    }

    // GET all viewing history for a profile
    [HttpGet("{profileId}")]
    public async Task<ActionResult<IEnumerable<ViewingHistoryDto>>> GetHistory(int profileId)
    {
        var histories = await _service.GetHistoryAsync(profileId);
        return Ok(histories);
    }

    // POST start viewing content
    [HttpPost]
    public async Task<ActionResult<ViewingHistoryDto>> StartViewing(ViewingHistoryDto request)
    {
        var newHistory = await _service.StartViewingAsync(request);
        return CreatedAtAction(nameof(GetHistory), new { profileId = newHistory.ProfileId }, newHistory);
    }

    // PUT update progress for a viewing history entry
    [HttpPut("{viewingHistoryId}")]
    public async Task<IActionResult> UpdateProgress(int viewingHistoryId, UpdateViewingHistoryDto request)
    {
        var updated = await _service.UpdateProgressAsync(viewingHistoryId, request);
        if (!updated) return NotFound();

        return NoContent();
    }

    // POST mark viewing as completed
    [HttpPost("{viewingHistoryId}/complete")]
    public async Task<IActionResult> MarkAsCompleted(int viewingHistoryId)
    {
        var updated = await _service.MarkAsCompletedAsync(viewingHistoryId);
        if (!updated) return NotFound();

        return NoContent();
    }

    // GET resume content (fetch last position to continue watching)
    [HttpGet("resume/{profileId}/{contentId}")]
    public async Task<ActionResult<ViewingHistoryDto?>> ResumeContent(int profileId, int contentId)
    {
        var history = await _service.ResumeContentAsync(profileId, contentId);
        if (history == null) return NotFound();

        return Ok(history);
    }
}
