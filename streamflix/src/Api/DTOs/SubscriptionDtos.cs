using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public class CreateSubscriptionDto
{
    [Required]
    [MaxLength(50, ErrorMessage = "SubscriptionType cannot be longer than 50 characters.")]
    public string SubscriptionType { get; set; }

    [Required]
    [MaxLength(200, ErrorMessage = "SubscriptionDescription cannot be longer than 200 characters.")]
    public string SubscriptionDescription { get; set; }

    public CreateSubscriptionDto()
    {
        SubscriptionType = string.Empty;
        SubscriptionDescription = string.Empty;
    }
}

public class ChangeSubscriptionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "SubscriptionId must be a positive integer.")]
    public int SubscriptionId { get; set; }

    [Required]
    [MaxLength(50, ErrorMessage = "NewSubscriptionType cannot be longer than 50 characters.")]
    public string NewSubscriptionType { get; set; }

    [Required]
    [MaxLength(200, ErrorMessage = "NewDescription cannot be longer than 200 characters.")]
    public string NewDescription { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "NewPrice must be zero or a positive value.")]
    public double NewPrice { get; set; }

    public ChangeSubscriptionDto()
    {
        NewSubscriptionType = string.Empty;
        NewDescription = string.Empty;
    }
}

public class CancelSubscriptionDto
{
    [Range(1, int.MaxValue, ErrorMessage = "SubscriptionId must be a positive integer.")]
    public int SubscriptionId { get; set; }
}

public class SubscriptionPlanDto
{
    public string Type { get; set; }
    public double Price { get; set; }

    public SubscriptionPlanDto()
    {
        Type = string.Empty;
    }
}

public class SubscriptionPlansDto
{
    public List<SubscriptionPlanDto> Plans { get; set; }

    public SubscriptionPlansDto()
    {
        Plans = new List<SubscriptionPlanDto>();
    }
}
