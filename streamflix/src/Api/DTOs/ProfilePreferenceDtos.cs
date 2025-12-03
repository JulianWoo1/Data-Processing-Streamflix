public record ProfilePreferenceDto(
    List<string> PreferredGenres,
    string ContentType,
    int MinimumAge,
    List<string> ContentFilters
);

public record UpdateProfilePreferenceDto(
    List<string> PreferredGenres,
    string ContentType,
    int MinimumAge,
    List<string> ContentFilters
);
