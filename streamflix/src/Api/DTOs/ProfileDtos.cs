using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public record CreateProfileDto(
    [Range(1, int.MaxValue, ErrorMessage = "AccountId must be a positive integer.")]
    int AccountId,

    [Required]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    string Name,

    [Required]
    [MaxLength(50, ErrorMessage = "AgeCategory cannot be longer than 50 characters.")]
    string AgeCategory,

    [Required]
    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    string ImageUrl
);

public record ProfileDto(
    int ProfileId,
    int AccountId,
    string Name,
    string AgeCategory,
    string ImageUrl,
    ProfilePreferenceDto? Preference,
    IEnumerable<ViewingHistoryDto> ViewingHistories
);

public record UpdateProfileDto(
    [Required]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    string Name,

    [Required]
    [MaxLength(50, ErrorMessage = "AgeCategory cannot be longer than 50 characters.")]
    string AgeCategory,

    [Required]
    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    string ImageUrl
);
