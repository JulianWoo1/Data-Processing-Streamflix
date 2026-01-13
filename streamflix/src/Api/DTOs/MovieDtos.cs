using System.ComponentModel.DataAnnotations;

namespace Streamflix.Api.DTOs;

public class MovieDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int AgeRating { get; set; }
    public string ImageURL { get; set; }
    public int Duration { get; set; }
    public string Genre { get; set; }
    public List<string> ContentWarnings { get; set; }
    public List<string> AvailableQualities { get; set; }

    public MovieDto()
    {
        Title = string.Empty;
        Description = string.Empty;
        ImageURL = string.Empty;
        Genre = string.Empty;
        ContentWarnings = new List<string>();
        AvailableQualities = new List<string>();
    }

    public MovieDto(int id, string title, string description, int ageRating, string imageURL, int duration, string genre, List<string> contentWarnings, List<string> availableQualities)
    {
        Id = id;
        Title = title;
        Description = description;
        AgeRating = ageRating;
        ImageURL = imageURL;
        Duration = duration;
        Genre = genre;
        ContentWarnings = contentWarnings;
        AvailableQualities = availableQualities;
    }
}

public class MovieRequestDto
{
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public int AgeRating { get; set; }
    public string ImageURL { get; set; }
    public int Duration { get; set; }
    public string Genre { get; set; }
    public List<string> ContentWarnings { get; set; }
    public List<string> AvailableQualities { get; set; }

    public MovieRequestDto()
    {
        Title = string.Empty;
        Description = string.Empty;
        ImageURL = string.Empty;
        Genre = string.Empty;
        ContentWarnings = new List<string>();
        AvailableQualities = new List<string>();
    }
}

public class MoviesDto
{
    public List<MovieDto> Movies { get; set; }

    public MoviesDto()
    {
        Movies = new List<MovieDto>();
    }
}
