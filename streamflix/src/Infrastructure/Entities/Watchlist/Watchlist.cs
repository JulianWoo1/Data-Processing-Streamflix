namespace Streamflix.Infrastructure.Entities;

public class Watchlist
{
    public int WatchlistId { get; set; }

    // Owner of the watchlist
    public int ProfileId { get; set; }

    // Navigation to the owning profile
    public Profile Profile { get; set; } = null!;

    // If you want a timestamp for when the watchlist as a whole was created
    public DateTime CreatedAt { get; set; }

    // 1‑many: one Watchlist has many WatchlistContent rows
    public List<WatchlistContent> Items { get; set; } = new();
}
