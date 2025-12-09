using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public record ViewingHistoryDto(
    [Range(1, int.MaxValue, ErrorMessage = "ViewingHistoryId must be a positive integer.")]
    int ViewingHistoryId,

    [Range(1, int.MaxValue, ErrorMessage = "ProfileId must be a positive integer.")]
    int ProfileId,

    [Range(1, int.MaxValue, ErrorMessage = "ContentId must be a positive integer.")]
    int ContentId,

    int? EpisodeId,
    DateTime StartTime,
    DateTime? EndTime,

    [Range(0, int.MaxValue, ErrorMessage = "LastPosition must be zero or a positive value.")]
    int LastPosition,

    bool IsCompleted
);

public record UpdateViewingHistoryDto(
    [Range(0, int.MaxValue, ErrorMessage = "LastPosition must be zero or a positive value.")]
    int LastPosition,
    bool IsCompleted
);
