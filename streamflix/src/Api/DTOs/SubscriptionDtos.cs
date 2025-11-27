namespace Streamflix.Api.DTOs;

public record CreateSubscriptionDto(
    int AccountId,
    string SubscriptionType,
    string SubscriptionDescription,
    double BasePrice
);

public record UpgradeSubscriptionDto(
    int SubscriptionId,
    string NewSubscriptionType,
    string NewSubscription,
    double NewPrice
);

public record CancelSubscriptionDto(
    int SubscriptionId
);