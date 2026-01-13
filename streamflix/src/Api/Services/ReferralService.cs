using Microsoft.EntityFrameworkCore;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;

namespace Streamflix.Api.Services;

public class ReferralService : IReferralService
{
    private readonly ApplicationDbContext _db;

    public ReferralService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Referral> CreateInvitationAsync(CreateInvitationDto dto)
    {
        var referral = new Referral
        {
            ReferrerAccountId = dto.ReferrerAccountId,
            InvitationCode = Guid.NewGuid().ToString("N"),
            IsActive = true,
            IsDiscountApplied = false
        };

        _db.Referrals.Add(referral);
        await _db.SaveChangesAsync();

        return referral;
    }

    public async Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto)
    {
        var referral = await _db.Referrals.FirstOrDefaultAsync(r => r.InvitationCode == dto.InvitationCode);
        if (referral == null || !referral.IsActive)
        {
            return false;
        }

        referral.ReferredAccountId = dto.ReferredAccountId;
        referral.AcceptDate = DateTime.UtcNow;
        referral.IsActive = false;

        if (!referral.IsDiscountApplied)
        {
            // Discount for the new user (referred)
            var referredSubscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.AccountId == dto.ReferredAccountId && s.IsActive);
            if (referredSubscription != null)
            {
                referredSubscription.BasePrice = Math.Max(0, referredSubscription.BasePrice - 2.50);
            }

            // Discount for the original user (referrer)
            var referrerSubscription = await _db.Subscriptions.FirstOrDefaultAsync(s => s.AccountId == referral.ReferrerAccountId && s.IsActive);
            if (referrerSubscription != null)
            {
                referrerSubscription.BasePrice = Math.Max(0, referrerSubscription.BasePrice - 2.50);
            }

            referral.IsDiscountApplied = true;
            referral.DiscountStartDate = DateTime.UtcNow;
            referral.DiscountEndDate = DateTime.UtcNow.AddMonths(1);
        }

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<string> GetReferralStatusAsync(string invitationCode)
    {
        var referral = await _db.Referrals.FirstOrDefaultAsync(r => r.InvitationCode == invitationCode);
        if (referral == null)
        {
            return "Referral not found";
        }

        return referral.IsActive ? "Pending acceptance" : "Accepted";
    }

    public async Task<object?> GetDiscountAsync(int accountId)
    {
        var referral = await _db.Referrals
            .Where(r => (r.ReferrerAccountId == accountId || r.ReferredAccountId == accountId)
                        && r.IsDiscountApplied)
            .OrderByDescending(r => r.DiscountEndDate)
            .FirstOrDefaultAsync();

        if (referral == null)
        {
            return null;
        }

        return new
        {
            referral.DiscountStartDate,
            referral.DiscountEndDate,
            DiscountAmount = 2.50
        };
    }
}
