namespace Streamflix.Api.DTOs;

public record CreateSubscriptionDto(
    int AccountId,
    string SubscriptionType,
    string SubscriptionDescription,
    double BasePrice
);

public record ChangeSubscriptionDto(
    int SubscriptionId,
    string NewSubscriptionType,
    string NewDescription,
    double NewPrice
);

public record CancelSubscriptionDto(
    int SubscriptionId
);
