namespace Streamflix.Infrastructure.Entities;

public class ProfilePreference
{
    public int ProfilePreferenceId { get; set; }
    public int ProfileId { get; set; }

    public string GenrePreference { get; set; } = null!;
    public bool PrefersMovies { get; set; }
    public bool PrefersSeries { get; set; }
    public int MinAgeAllowed { get; set; }
    public bool BlockViolence { get; set; }
    public bool BlockFear { get; set; }
    public bool BlockLanguage { get; set; }

    // Navigatie terug naar Profile
    public Profile Profile { get; set; } = null!;
}
