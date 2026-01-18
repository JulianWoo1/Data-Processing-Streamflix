using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public class CreateProfileDto
{
    [Range(1, int.MaxValue, ErrorMessage = "AccountId must be a positive integer.")]
    public int AccountId { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "AgeCategory cannot be longer than 50 characters.")]
    public string AgeCategory { get; set; }

    [Required]
    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    public string ImageUrl { get; set; }

    public CreateProfileDto()
    {
        Name = string.Empty;
        AgeCategory = string.Empty;
        ImageUrl = string.Empty;
    }
}

public class ProfileDto
{
    public int ProfileId { get; set; }
    public int AccountId { get; set; }
    public string Name { get; set; }
    public string AgeCategory { get; set; }
    public string ImageUrl { get; set; }
    public ProfilePreferenceDto? Preference { get; set; }
    public IEnumerable<ViewingHistoryDto> ViewingHistories { get; set; }

    public ProfileDto()
    {
        Name = string.Empty;
        AgeCategory = string.Empty;
        ImageUrl = string.Empty;
        ViewingHistories = new List<ViewingHistoryDto>();
    }

    public ProfileDto(int profileId, int accountId, string name, string ageCategory, string imageUrl, ProfilePreferenceDto? preference, IEnumerable<ViewingHistoryDto> viewingHistories)
    {
        ProfileId = profileId;
        AccountId = accountId;
        Name = name;
        AgeCategory = ageCategory;
        ImageUrl = imageUrl;
        Preference = preference;
        ViewingHistories = viewingHistories;
    }
}

public class UpdateProfileDto
{
    [Required]
    [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    public string Name { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "AgeCategory cannot be longer than 50 characters.")]
    public string AgeCategory { get; set; }

    [Required]
    [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
    public string ImageUrl { get; set; }

    public UpdateProfileDto()
    {
        Name = string.Empty;
        AgeCategory = string.Empty;
        ImageUrl = string.Empty;
    }
}

public class ProfilesDto
{
    public List<ProfileDto> Profiles { get; set; }

    public ProfilesDto()
    {
        Profiles = new List<ProfileDto>();
    }
}
