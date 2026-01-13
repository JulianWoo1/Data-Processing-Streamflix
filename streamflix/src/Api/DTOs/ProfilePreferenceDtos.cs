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
    public List<string> PreferredGenres { get; set; }
    public string ContentType { get; set; }
    public int MinimumAge { get; set; }
    public List<string> ContentFilters { get; set; }

    public UpdateProfilePreferenceDto()
    {
        PreferredGenres = new List<string>();
        ContentType = string.Empty;
        ContentFilters = new List<string>();
    }
}
