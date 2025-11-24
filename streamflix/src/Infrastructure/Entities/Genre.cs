using Streamflix.Infrastructure.Enums;

namespace Streamflix.Infrastructure.Entities;

public class Genre
{
    public int GenreId { get; set; }
    public string GenreType { get; set; }

    // 1 Genre -> Many Contents
    public List<Content> Contents { get; set; } = new();
}
