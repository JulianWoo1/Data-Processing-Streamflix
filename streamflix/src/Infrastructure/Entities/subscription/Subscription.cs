namespace Streamflix.Infrastructure.Entities;

public class Subscription
{
    public int SubscriptionId { get; set; }
    public int AccountId { get; set; }
    public string SubscriptionType { get; set; }
    public string SubscriptionDescription { get; set; }
    public double BasePrice { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsTrialPeriod  { get; set; }
    public DateTime? TrialPeriodEnd  { get; set; }
}
