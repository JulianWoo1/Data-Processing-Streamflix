using Streamflix.Infrastructure.Enums;

namespace Streamflix.Api.DTOs;

public record CreateMovieDto(
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    int Duration,
    GenreType Genre,
    List<ContentWarningType> ContentWarnings,
    List<QualityType> AvailableQualities
);

public record MovieDto(
    int Id,
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    int Duration,
    string Genre,
    List<string> ContentWarnings,
    List<string> AvailableQualities
);
