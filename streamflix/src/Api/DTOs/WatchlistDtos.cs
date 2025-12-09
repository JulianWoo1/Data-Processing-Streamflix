using System.ComponentModel.DataAnnotations;
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
    [Range(1, int.MaxValue, ErrorMessage = "ProfileId must be a positive integer.")]
    int ProfileId,

    [Range(1, int.MaxValue, ErrorMessage = "ContentId must be a positive integer.")]
    int ContentId
);
