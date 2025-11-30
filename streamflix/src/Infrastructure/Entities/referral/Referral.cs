namespace Streamflix.Infrastructure.Entities;

public class Referral
{
    public int ReferralId { get; set; }
    public int accountId { get; set; }
    public string subscriptionType { get; set;}
    public double BasePrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive {get; set; }
    public bool IsTrialPeriod { get; set; }
    public DateTime TrialPeriodEnd {get; set; }
}