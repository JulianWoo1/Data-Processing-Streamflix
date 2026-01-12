using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public class WatchlistDto
{
    public int WatchlistId { get; set; }
    public int ProfileId { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<WatchlistContentDto> Items { get; set; }

    public WatchlistDto()
    {
        Items = new List<WatchlistContentDto>();
    }

    public WatchlistDto(int watchlistId, int profileId, DateTime createdAt, List<WatchlistContentDto> items)
    {
        WatchlistId = watchlistId;
        ProfileId = profileId;
        CreatedAt = createdAt;
        Items = items;
    }
}

public class WatchlistContentDto
{
    public int WatchlistContentId { get; set; }
    public int ContentId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int AgeRating { get; set; }
    public string ImageURL { get; set; }
    public string Genre { get; set; }
    public List<string> ContentWarnings { get; set; }
    public List<string> AvailableQualities { get; set; }
    public DateTime DateAdded { get; set; }

    public WatchlistContentDto()
    {
        Title = string.Empty;
        Description = string.Empty;
        ImageURL = string.Empty;
        Genre = string.Empty;
        ContentWarnings = new List<string>();
        AvailableQualities = new List<string>();
    }
}

public class AddToWatchListRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "ProfileId must be a positive integer.")]
    public int ProfileId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ContentId must be a positive integer.")]
    public int ContentId { get; set; }
}
