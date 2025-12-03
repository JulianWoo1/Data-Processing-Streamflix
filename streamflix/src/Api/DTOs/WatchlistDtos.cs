namespace Streamflix.Api.DTOs;

public record WatchlistDto(
    int WatchlistId,
    int ProfileId,
    DateTime CreatedAt,
    List<WatchlistContentDto> Items
);

public record WatchlistContentDto(
    int WatchlistContentId,
    int ContentId,
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    string Genre,
    List<string> ContentWarnings,
    List<string> AvailableQualities,
    DateTime DateAdded
);

public record AddToWatchListRequest(
    int ProfileId,
    int ContentId
);
