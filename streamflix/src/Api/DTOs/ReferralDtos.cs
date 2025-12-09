using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public record CreateInvitationDto(
    [Range(1, int.MaxValue, ErrorMessage = "ReferrerAccountId must be a positive integer.")]
    int ReferrerAccountId
);

public record AcceptInvitationDto(
    [Required]
    [MaxLength(50, ErrorMessage = "InvitationCode cannot be longer than 50 characters.")]
    string InvitationCode,

    [Range(1, int.MaxValue, ErrorMessage = "ReferredAccountId must be a positive integer.")]
    int ReferredAccountId
);
