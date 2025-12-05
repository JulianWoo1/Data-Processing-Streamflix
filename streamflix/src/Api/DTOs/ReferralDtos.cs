namespace Streamflix.Api.DTOs;

public record CreateInvitationDto(
    int ReferralId,
    int accountId,
    string subscriptionType, 
    double BasePrice,
    DateTime StartDate,
    DateTime EndDate, 
    bool IsActive, 
    bool IsTrialPeriod, 
    DateTime TrialPeriodEnd 
);

public record AcceptInvitation(
    int ReferralId  
);

public record GetReferralStatus(

);

public record GetDiscount(

);