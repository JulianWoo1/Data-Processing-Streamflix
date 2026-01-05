using System.ComponentModel.DataAnnotations;

namespace Streamflix.Api.DTOs;

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
    int Id,
    int SeasonNumber,
    int TotalEpisodes,
    List<EpisodeDto> Episodes
);

public record EpisodeDto(
    int Id,
    int EpisodeNumber,
    string Title,
    int Duration
);

public record SeriesRequestDto(
    [Required]
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    string Genre,
    List<string> ContentWarnings,
    List<string> AvailableQualities
    // Seasons and episodes will be managed via separate endpoints for clarity
);

public record SeasonRequestDto(
    [Required]
    int SeasonNumber
);

public record EpisodeRequestDto(
    [Required]
    int EpisodeNumber,
    [Required]
    string Title,
    int Duration
);
