using System.ComponentModel.DataAnnotations;

namespace Streamflix.Api.DTOs;

public class SeriesDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int AgeRating { get; set; }
    public string ImageURL { get; set; }
    public int TotalSeasons { get; set; }
    public string Genre { get; set; }
    public List<string> ContentWarnings { get; set; }
    public List<string> AvailableQualities { get; set; }
    public List<SeasonDto> Seasons { get; set; }

    public SeriesDto()
    {
        Title = string.Empty;
        Description = string.Empty;
        ImageURL = string.Empty;
        Genre = string.Empty;
        ContentWarnings = new List<string>();
        AvailableQualities = new List<string>();
        Seasons = new List<SeasonDto>();
    }

    public SeriesDto(int id, string title, string description, int ageRating, string imageURL, int totalSeasons, string genre, List<string> contentWarnings, List<string> availableQualities, List<SeasonDto> seasons)
    {
        Id = id;
        Title = title;
        Description = description;
        AgeRating = ageRating;
        ImageURL = imageURL;
        TotalSeasons = totalSeasons;
        Genre = genre;
        ContentWarnings = contentWarnings;
        AvailableQualities = availableQualities;
        Seasons = seasons;
    }
}

public class SeasonDto
{
    public int Id { get; set; }
    public int SeasonNumber { get; set; }
    public int TotalEpisodes { get; set; }
    public List<EpisodeDto> Episodes { get; set; }

    public SeasonDto()
    {
        Episodes = new List<EpisodeDto>();
    }

    public SeasonDto(int id, int seasonNumber, int totalEpisodes, List<EpisodeDto> episodes)
    {
        Id = id;
        SeasonNumber = seasonNumber;
        TotalEpisodes = totalEpisodes;
        Episodes = episodes;
    }
}

public class EpisodeDto
{
    public int Id { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; }
    public int Duration { get; set; }

    public EpisodeDto()
    {
        Title = string.Empty;
    }

    public EpisodeDto(int id, int episodeNumber, string title, int duration)
    {
        Id = id;
        EpisodeNumber = episodeNumber;
        Title = title;
        Duration = duration;
    }
}

public class SeriesRequestDto
{
    [Required]
    [MaxLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
    public string Title { get; set; }

    [Required]
    [MaxLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
    public string Description { get; set; }

    [Required]
    [Range(0, 21, ErrorMessage = "Age rating must be between 0 and 21.")]
    public int AgeRating { get; set; }

    public string ImageURL { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Genre cannot be longer than 100 characters.")]
    public string Genre { get; set; }

    [Required]
    public List<string> ContentWarnings { get; set; }

    [Required]
    public List<string> AvailableQualities { get; set; }

    public SeriesRequestDto()
    {
        Title = string.Empty;
        Description = string.Empty;
        ImageURL = string.Empty;
        Genre = string.Empty;
        ContentWarnings = new List<string>();
        AvailableQualities = new List<string>();
    }
}

public class SeasonRequestDto
{
    [Required]
    [Range(1, 50, ErrorMessage = "Season number must be between 1 and 50.")]
    public int SeasonNumber { get; set; }
}

public class EpisodeRequestDto
{
    [Required]
    [Range(1, 500, ErrorMessage = "Episode number must be between 1 and 500.")]
    public int EpisodeNumber { get; set; }

    [Required]
    [MaxLength(200, ErrorMessage = "Episode title cannot be longer than 200 characters.")]
    public string Title { get; set; }

    [Range(0, 300, ErrorMessage = "Duration must be between 0 and 300 minutes.")]
    public int Duration { get; set; }

    public EpisodeRequestDto()
    {
        Title = string.Empty;
    }
}

public class SeriesListDto
{
    public List<SeriesDto> Series { get; set; }

    public SeriesListDto()
    {
        Series = new List<SeriesDto>();
    }
}
