using Streamflix.Infrastructure.Entities;  
using Streamflix.Api.Settings;            
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Streamflix.Api.DTOs;

namespace Streamflix.Api.Services;

public interface IreferralService
{
    Task<Referral> CreateInvitationAsync(CreateAccountDto dto);
    Task<bool> AcceptInvitationAsync(int referralId);
    Task<string> GetReferralStatusAsync(int referralId);
    Task<double> GetDiscountAsync(int referralId);
}