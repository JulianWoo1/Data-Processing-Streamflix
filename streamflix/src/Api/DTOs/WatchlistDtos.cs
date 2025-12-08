namespace Streamflix.Api.DTOs;

// TODO: voeg ContentDto later toe
public record WatchlistDto(
    int WatchlistId,
    int ProfileId,
    DateTime DateAdded
);

public record UpdateWatchlistDto(
    List<int> ContentIds
);
