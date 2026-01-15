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
    [MaxLength(200, ErrorMessage = "Title cannot be longer than 200 characters.")]
    public string Title { get; set; }

    [Required]
    [MaxLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
    public string Description { get; set; }

    [Required]
    [Range(0, 21, ErrorMessage = "Age rating must be between 0 and 21.")]
    public int AgeRating { get; set; }

    public string ImageURL { get; set; }

    [Range(0, 600, ErrorMessage = "Duration must be between 0 and 600 minutes.")]
    public int Duration { get; set; }

    [Required]
    [MaxLength(100, ErrorMessage = "Genre cannot be longer than 100 characters.")]
    public string Genre { get; set; }

    public List<string> ContentWarnings { get; set; }

    [Required]
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
