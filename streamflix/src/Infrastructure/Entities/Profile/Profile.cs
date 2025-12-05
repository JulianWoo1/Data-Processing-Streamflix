namespace Streamflix.Infrastructure.Entities;

public class Profile
{
    public int ProfileId { get; set; }
    public int AccountId { get; set; }

    public string Name { get; set; } = null!;
    public string AgeCategory { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;

    // Relatie met ProfilePreference (one-to-one)
    public ProfilePreference? Preference { get; set; }

    // One Profile has many Watchlists
    public List<Watchlist> Watchlists { get; set; } = new();
}
