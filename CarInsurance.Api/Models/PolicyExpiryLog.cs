namespace CarInsurance.Api.Models;
public class PolicyExpiryLog
{
    public long Id { get; set; }
    
    public long PolicyId { get; set; }
    public InsurancePolicy Policy { get; set; } = default!;

    public DateOnly EndDate { get; set; }
    public DateTime LoggedAt { get; set; }
}
