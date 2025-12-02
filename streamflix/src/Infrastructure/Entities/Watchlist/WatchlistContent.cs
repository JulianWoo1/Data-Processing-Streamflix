namespace Streamflix.Infrastructure.Entities;

public class WatchlistContent
{
    public int WatchlistContentId { get; set; }

    // FK to Watchlist
    public int WatchlistId { get; set; }
    public Watchlist Watchlist { get; set; } = null!;

    // FK to Content
    public int ContentId { get; set; }
    public Content Content { get; set; } = null!;

    // Per-item metadata
    public DateTime DateAdded { get; set; }
}
