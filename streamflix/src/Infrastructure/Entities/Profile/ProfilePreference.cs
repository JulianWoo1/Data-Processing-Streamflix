namespace Streamflix.Infrastructure.Entities
{
    public class ProfilePreference
    {
        public int ProfilePreferenceId { get; set; }
        public int ProfileId { get; set; }

        public List<string> PreferredGenres { get; set; } = new();
        public string ContentType { get; set; } = null!;
        public int MinimumAge { get; set; }
        public List<string> ContentFilters { get; set; } = new();

        // Relatie met Watchlist (one-to-many)
        // public List<Watchlist> Watchlists { get; set; } = new();

        // Relatie met Profile (one-to-one)
        public Profile Profile { get; set; } = null!;
    }
}
