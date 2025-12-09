namespace Streamflix.Infrastructure.Entities;

public class Referral
{
    public int ReferralId { get; set; }

    public int ReferrerAccountId { get; set; }     
    public int? ReferredAccountId { get; set; }     

    public string InvitationCode { get; set; } = null!;
    public DateTime InvitationDate { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptDate { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsDiscountApplied { get; set; } = false;

    public DateTime? DiscountStartDate { get; set; }
    public DateTime? DiscountEndDate { get; set; }
}
