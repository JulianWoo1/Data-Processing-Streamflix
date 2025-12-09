namespace Streamflix.Api.DTOs;

public record CreateInvitationDto(
    int ReferrerAccountId
);

public record AcceptInvitationDto(
    string InvitationCode,
    int ReferredAccountId
);
