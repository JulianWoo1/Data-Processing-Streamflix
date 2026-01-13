namespace Streamflix.Infrastructure.Entities;

public class ViewingHistory
{
    public int ViewingHistoryId { get; set; } // Primary key
    public int ProfileId { get; set; }

    public int ContentId { get; set; }
    public int? EpisodeId { get; set; } // nullable, want niet elke content heeft episodes

    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; } // nullable, kan nog bezig zijn
    public int LastPosition { get; set; }
    public bool IsCompleted { get; set; }

    // Relatie met Profile (many-to-one)
    public Profile Profile { get; set; } = null!;
}
