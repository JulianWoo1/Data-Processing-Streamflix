namespace Streamflix.Infrastructure.Entities;

public class Episode
{
    public int EpisodeId { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; } = null!;
    public int Duration { get; set; }

    // Relatie naar Season (many-to-one)
    public int SeasonId { get; set; }
    public Season Season { get; set; } = null!;
}
