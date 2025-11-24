using Streamflix.Infrastructure.Enums;

namespace Streamflix.Api.DTOs;

public record CreateSeriesDto(
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    int TotalSeasons,
    string Genre,
    List<string> ContentWarnings,
    List<string> AvailableQualities
);

public record SeriesDto(
    int Id,
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    int TotalSeasons,
    string Genre,
    List<string> ContentWarnings,
    List<string> AvailableQualities,
    List<SeasonDto> Seasons
);

public record SeasonDto(
    int SeasonId,
    int SeasonNumber,
    int TotalEpisodes,
    List<EpisodeDto> Episodes
);

public record EpisodeDto(
    int EpisodeId,
    int EpisodeNumber,
    string Title,
    int Duration
);
