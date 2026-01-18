using System.ComponentModel.DataAnnotations;

namespace Streamflix.Api.DTOs;

public class ProfilePreferenceDto
{
    public List<string> PreferredGenres { get; set; }
    public string ContentType { get; set; }
    public int MinimumAge { get; set; }
    public List<string> ContentFilters { get; set; }

    public ProfilePreferenceDto()
    {
        PreferredGenres = new List<string>();
        ContentType = string.Empty;
        ContentFilters = new List<string>();
    }

    public ProfilePreferenceDto(List<string> preferredGenres, string contentType, int minimumAge, List<string> contentFilters)
    {
        PreferredGenres = preferredGenres;
        ContentType = contentType;
        MinimumAge = minimumAge;
        ContentFilters = contentFilters;
    }
}

public class UpdateProfilePreferenceDto
{
    [Required]
    public List<string> PreferredGenres { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "ContentType cannot be longer than 50 characters.")]
    public string ContentType { get; set; }

    [Required]
    [Range(0, 21, ErrorMessage = "MinimumAge must be between 0 and 21.")]
    public int MinimumAge { get; set; }

    [Required]
    public List<string> ContentFilters { get; set; }

    public UpdateProfilePreferenceDto()
    {
        PreferredGenres = new List<string>();
        ContentType = string.Empty;
        ContentFilters = new List<string>();
    }
}
