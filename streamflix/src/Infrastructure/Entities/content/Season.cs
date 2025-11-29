namespace Streamflix.Infrastructure.Entities;

public class Season
{
    public int SeasonId { get; set; }
    public int SeasonNumber { get; set; }
    public int TotalEpisodes { get; set; }

    // Relatie naar Series (many-to-one)
    public int SeriesId { get; set; }
    public Series Series { get; set; } = null!;

    // 1 Season -> Many Episodes
    public List<Episode> Episodes { get; set; } = new();
}