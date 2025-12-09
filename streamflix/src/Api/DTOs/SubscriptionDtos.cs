using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public record CreateSubscriptionDto(
    [Range(1, int.MaxValue, ErrorMessage = "AccountId must be a positive integer.")]
    int AccountId,

    [Required]
    [MaxLength(50, ErrorMessage = "SubscriptionType cannot be longer than 50 characters.")]
    string SubscriptionType,

    [Required]
    [MaxLength(200, ErrorMessage = "SubscriptionDescription cannot be longer than 200 characters.")]
    string SubscriptionDescription,

    [Range(0, double.MaxValue, ErrorMessage = "BasePrice must be zero or a positive value.")]
    double BasePrice
);

public record ChangeSubscriptionDto(
    [Range(1, int.MaxValue, ErrorMessage = "SubscriptionId must be a positive integer.")]
    int SubscriptionId,

    [Required]
    [MaxLength(50, ErrorMessage = "NewSubscriptionType cannot be longer than 50 characters.")]
    string NewSubscriptionType,

    [Required]
    [MaxLength(200, ErrorMessage = "NewDescription cannot be longer than 200 characters.")]
    string NewDescription,

    [Range(0, double.MaxValue, ErrorMessage = "NewPrice must be zero or a positive value.")]
    double NewPrice
);

public record CancelSubscriptionDto(
    [Range(1, int.MaxValue, ErrorMessage = "SubscriptionId must be a positive integer.")]
    int SubscriptionId
);
