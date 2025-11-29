namespace Streamflix.Infrastructure.Entities;

public class Series : Content
{
    public int TotalSeasons { get; set; }

    // 1 Series -> Many Seasons
    public List<Season> Seasons { get; set; } = new();
}