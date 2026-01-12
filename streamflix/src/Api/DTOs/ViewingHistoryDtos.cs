using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public class CreateViewingHistoryDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ProfileId must be a positive integer.")]
    public int ProfileId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ContentId must be a positive integer.")]
    public int ContentId { get; set; }

    public int? EpisodeId { get; set; }
}

public class ViewingHistoryDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ViewingHistoryId must be a positive integer.")]
    public int ViewingHistoryId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ProfileId must be a positive integer.")]
    public int ProfileId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ContentId must be a positive integer.")]
    public int ContentId { get; set; }

    public int? EpisodeId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "LastPosition must be zero or a positive value.")]
    public int LastPosition { get; set; }

    public bool IsCompleted { get; set; }

    public ViewingHistoryDto() { }

    public ViewingHistoryDto(int viewingHistoryId, int profileId, int contentId, int? episodeId, DateTime startTime, DateTime? endTime, int lastPosition, bool isCompleted)
    {
        ViewingHistoryId = viewingHistoryId;
        ProfileId = profileId;
        ContentId = contentId;
        EpisodeId = episodeId;
        StartTime = startTime;
        EndTime = endTime;
        LastPosition = lastPosition;
        IsCompleted = isCompleted;
    }
}

public class UpdateViewingHistoryDto
{
    [Range(0, int.MaxValue, ErrorMessage = "LastPosition must be zero or a positive value.")]
    public int LastPosition { get; set; }
    public bool IsCompleted { get; set; }
}

public class ViewingHistoriesDto
{
    public List<ViewingHistoryDto> ViewingHistories { get; set; }

    public ViewingHistoriesDto()
    {
        ViewingHistories = new List<ViewingHistoryDto>();
    }
}
