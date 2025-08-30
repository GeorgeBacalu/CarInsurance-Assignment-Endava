namespace CarInsurance.Api.Dtos.Requests;
public record AddInsuranceClaimRequest(DateOnly ClaimDate, string Description, decimal Amount);
