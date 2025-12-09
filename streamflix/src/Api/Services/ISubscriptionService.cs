using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;

public interface ISubscriptionService
{
    Task<Subscription?> GetSubscriptionAsync(int accountId);
    Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionDto dto);
    Task<Subscription?> ChangeSubscriptionAsync(ChangeSubscriptionDto dto);
    Task<bool> CancelSubscriptionAsync(int subscriptionId);
    Task<Subscription?> RenewSubscriptionAsync(int subscriptionId);
}
