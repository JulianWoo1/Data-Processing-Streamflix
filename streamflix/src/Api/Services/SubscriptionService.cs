using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;

namespace Streamflix.Api.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ApplicationDbContext _db;
    private static readonly Dictionary<string, double> Prices = new()
    {
        { "SD", 7.99 },
        { "HD", 10.99 },
        { "UHD", 13.99 }
    };

    public SubscriptionService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Subscription?> GetSubscriptionAsync(int accountId)
    {
        return await _db.Subscriptions
            .FirstOrDefaultAsync(s => s.AccountId == accountId && s.IsActive);
    }

    public async Task<Subscription> CreateSubscriptionAsync(int accountId, CreateSubscriptionDto dto)
    {
        var type = dto.SubscriptionType.ToUpper();

        if (!Prices.ContainsKey(type))
        {
            throw new ArgumentException("Invalid subscription type. Allowed: SD, HD, UHD.");            
        }

        var subscription = new Subscription
        {
            AccountId = dto.AccountId,
            SubscriptionType = type,
            SubscriptionDescription = dto.SubscriptionDescription,
            BasePrice = Prices[type],

            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),

            IsActive = true,
            IsTrialPeriod = true,
            TrialPeriodEnd = DateTime.UtcNow.AddDays(7),
        };

        _db.Subscriptions.Add(subscription);
        await _db.SaveChangesAsync();

        return subscription;
    }

    public async Task<Subscription?> ChangeSubscriptionAsync(int accountId, int subscriptionId, ChangeSubscriptionDto dto)
    {
        var subscription = await _db.Subscriptions
            .FirstOrDefaultAsync(s =>
                s.SubscriptionId == subscriptionId &&
                s.AccountId == accountId &&
                s.IsActive);

        if (subscription == null || !subscription.IsActive)
        {
            return null;            
        }

        var type = dto.NewSubscriptionType.ToUpper();

        if (!Prices.ContainsKey(type))
        {
            throw new ArgumentException("Invalid subscription type. Allowed: SD, HD, UHD.");            
        }

        subscription.SubscriptionType = type;
        subscription.SubscriptionDescription = dto.NewDescription;
        subscription.BasePrice = Prices[type];

        subscription.IsTrialPeriod = false;
        subscription.TrialPeriodEnd = null;

        subscription.EndDate = DateTime.UtcNow.AddMonths(1);

        await _db.SaveChangesAsync();
        return subscription;
    }

    public async Task<bool> CancelSubscriptionAsync(int accountId, int subscriptionId)
    {
        var subscription = await _db.Subscriptions
            .FirstOrDefaultAsync(s =>
            s.SubscriptionId == subscriptionId &&
            s.AccountId == accountId && 
            s.IsActive);

        if (subscription == null)
        {
            return false;            
        }

        subscription.IsActive = false;
        subscription.EndDate = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Subscription?> RenewSubscriptionAsync(int accoundtId, int subscriptionId)
    {
        var subscription = await _db.Subscriptions
            .FirstOrDefaultAsync(s =>
                s.SubscriptionId == subscriptionId &&
                s.AccountId == accoundtId &&
                s.IsActive);

        if (subscription == null || !subscription.IsActive)
        {
            return null;
        }

        if (subscription.IsTrialPeriod && subscription.TrialPeriodEnd > DateTime.UtcNow)
        {
            subscription.IsTrialPeriod = false;
            subscription.TrialPeriodEnd = null;
        }

        subscription.EndDate = DateTime.UtcNow.AddMonths(1);

        await _db.SaveChangesAsync();
        return subscription;
    }
}
