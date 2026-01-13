using System.ComponentModel.DataAnnotations;
namespace Streamflix.Api.DTOs;

public class CreateInvitationDto
{
    [Range(1, int.MaxValue, ErrorMessage = "ReferrerAccountId must be a positive integer.")]
    public int ReferrerAccountId { get; set; }
}

public class AcceptInvitationDto
{
    [Required]
    [MaxLength(50, ErrorMessage = "InvitationCode cannot be longer than 50 characters.")]
    public string InvitationCode { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ReferredAccountId must be a positive integer.")]
    public int ReferredAccountId { get; set; }

    public AcceptInvitationDto()
    {
        InvitationCode = string.Empty;
    }
}
