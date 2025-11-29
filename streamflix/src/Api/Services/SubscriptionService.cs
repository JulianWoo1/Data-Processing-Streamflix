namespace Streamflix.Api.Services;

using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _db;

    public SubscriptionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Subscription?> GetSubscriptionAsync(int accountId)
    {
        return await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.AccountId == accountId && s.IsActive);
    }

    public async Task<Subscription> CreateSubscriptionAsync(CreateSubscriptionDto dto)
    {
        var subscription = new Subscription
        {
            AccountId = dto.AccountId,
            SubscriptionType = dto.SubscriptionType,
            SubscriptionDescription = dto.SubscriptionDescription,
            BasePrice = dto.BasePrice,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
            IsTrialPeriod = true,
            TrialPeriodEnd = DateTime.UtcNow.AddDays(7)
        };

        _db.Subscriptions.Add(subscription);
        await _db.SaveChangesAsync();

        return subscription;
    }

    public async Task<Subscription?> ChangeSubscriptionAsync(ChangeSubscriptionDto dto)
    {
        var subscription = await _db.Subscriptions.FindAsync(dto.SubscriptionId);
        if (subscription == null || !subscription.IsActive) return null;

        subscription.SubscriptionType = dto.NewSubscriptionType;
        subscription.SubscriptionDescription = dto.NewDescription;
        subscription.BasePrice = dto.NewPrice;
        subscription.EndDate = DateTime.UtcNow.AddMonths(1);
        subscription.IsTrialPeriod = false;
        subscription.TrialPeriodEnd = null;

        await _db.SaveChangesAsync();
        return subscription;
    }

    public async Task<bool> CancelSubscriptionAsync(int subscriptionId)
    {
        var subscription = await _db.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null) return false;

        subscription.IsActive = false;
        subscription.EndDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<Subscription?> RenewSubscriptionAsync(int subscriptionId)
    {
        var subscription = await _db.Subscriptions.FindAsync(subscriptionId);
        if (subscription == null || !subscription.IsActive) return null;

        subscription.EndDate = DateTime.UtcNow.AddMonths(1);
        await _db.SaveChangesAsync();

        return subscription;
    }
}
