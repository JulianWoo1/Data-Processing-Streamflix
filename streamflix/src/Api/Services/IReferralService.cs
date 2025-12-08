using Streamflix.Infrastructure.Entities;
using Streamflix.Api.DTOs;

namespace Streamflix.Api.Services;

public interface IReferralService
{
    Task<Referral> CreateInvitationAsync(CreateInvitationDto dto);
    Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto);
    Task<string> GetReferralStatusAsync(string invitationCode);
    Task<object?> GetDiscountAsync(int accountId);
}
