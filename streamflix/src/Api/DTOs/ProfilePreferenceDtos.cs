public record ProfilePreferenceDto(
    string GenrePreference,
    bool PrefersMovies,
    bool PrefersSeries,
    int MinAgeAllowed,
    bool BlockViolence,
    bool BlockFear,
    bool BlockLanguage
);

public record UpdateProfilePreferenceDto(
    string GenrePreference,
    bool PrefersMovies,
    bool PrefersSeries,
    int MinAgeAllowed,
    bool BlockViolence,
    bool BlockFear,
    bool BlockLanguage
);
