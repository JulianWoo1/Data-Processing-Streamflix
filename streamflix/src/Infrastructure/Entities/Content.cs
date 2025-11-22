namespace Streamflix.Infrastructure.Entities;

public abstract class Content
{
    public int ContentId { get; set; }

    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int AgeRating { get; set; }
    public string ImageURL { get; set; } = null!;

    // Genre (many-to-one)
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;

    // ContentWarnings (many-to-many)
    public List<ContentWarning> ContentWarnings { get; set; } = new();

    // List<Quality>
    public List<Quality> AvailableQualities { get; set; } = new();
}
