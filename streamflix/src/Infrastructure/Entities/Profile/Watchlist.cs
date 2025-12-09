namespace Streamflix.Infrastructure.Entities;

public class Watchlist
{
    public int WatchlistId { get; set; }
    public int ProfileId { get; set; }
    public DateTime DateAdded { get; set; }

    // Eén Watchlist heeft meerdere Content-items
    public List<Content> Content { get; set; } = new();

    // Relatie met Profile (one-to-one)
    public Profile Profile { get; set; } = null!;
}


