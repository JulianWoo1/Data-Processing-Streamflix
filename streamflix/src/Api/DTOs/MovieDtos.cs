using System.ComponentModel.DataAnnotations;

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

public record MovieRequestDto(
    [Required]
    string Title,
    string Description,
    int AgeRating,
    string ImageURL,
    int Duration,
    string Genre,
    List<string> ContentWarnings,
    List<string> AvailableQualities
);
