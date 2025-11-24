using Streamflix.Infrastructure.Enums;

namespace Streamflix.Infrastructure.Entities;

public class ContentWarning
{
    public int ContentWarningId { get; set; }
    public string ContentWarningType { get; set; }

    // Relatie many-to-many
    public List<Content> Contents { get; set; } = new();
}
