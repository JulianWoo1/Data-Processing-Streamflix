namespace Streamflix.Infrastructure.Entities;

public class Quality
{
    public int QualityId { get; set; }
    public string QualityType { get; set; }

    // 1 Quality -> Many Contents
    public List<Content> Contents { get; set; } = new();
}
