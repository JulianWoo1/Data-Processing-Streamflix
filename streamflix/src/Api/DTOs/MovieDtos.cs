namespace Streamflix.Api.DTOs;

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
