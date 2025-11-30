namespace Streamflix.Api.DTOs;

public record CreateProfileDto(
    int AccountId,
    string Name,
    string AgeCategory,
    string ImageUrl
);

public record ProfileDto(
    int ProfileId,
    int AccountId,
    string Name,
    string AgeCategory,
    string ImageUrl,
    ProfilePreferenceDto? Preferences
);

public record UpdateProfileDto(
    string Name,
    string AgeCategory,
    string ImageUrl
);
