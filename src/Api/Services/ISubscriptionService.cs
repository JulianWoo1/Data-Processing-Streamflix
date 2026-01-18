using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;

public interface ISubscriptionService
{
    Task<Subscription?> GetSubscriptionAsync(int accountId);
    Task<Subscription> CreateSubscriptionAsync(int accountId, CreateSubscriptionDto dto);
    Task<Subscription?> ChangeSubscriptionAsync(int accountId, int subscriptionId, ChangeSubscriptionDto dto);
    Task<bool> CancelSubscriptionAsync(int accountId, int subscriptionId);
    Task<Subscription?> RenewSubscriptionAsync(int accountId, int subscriptionId);
}
