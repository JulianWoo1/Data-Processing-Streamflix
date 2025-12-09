namespace Streamflix.Api.DTOs;

public record ViewingHistoryDto(
    int ViewingHistoryId,
    int ProfileId,
    int ContentId,
    int? EpisodeId,
    DateTime StartTime,
    DateTime? EndTime,
    int LastPosition,
    bool IsCompleted
);

public record UpdateViewingHistoryDto(
    int LastPosition,
    bool IsCompleted
);
