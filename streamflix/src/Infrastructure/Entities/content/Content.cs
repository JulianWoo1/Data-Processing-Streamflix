namespace Streamflix.Infrastructure.Entities;

public abstract class Content
{
    public int ContentId { get; set; }

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int AgeRating { get; set; }
    public string ImageURL { get; set; } = null!;

    // Genre
    public string Genre { get; set; } = null!;

    // ContentWarnings
    public List<string> ContentWarnings { get; set; } = new();

    // AvailableQualities
    public List<string> AvailableQualities { get; set; } = new();
}
