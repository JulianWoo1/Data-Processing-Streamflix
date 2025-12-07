namespace Streamflix.Infrastructure.Entities;

public class Discount
{
    public int DiscountId { get; set; }
    public int AccountId { get; set; }
    public double DiscountAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
