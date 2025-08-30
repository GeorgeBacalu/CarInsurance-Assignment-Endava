namespace CarInsurance.Api.Dtos.Common;
public record InsuranceClaimDto(long Id, DateOnly Date, string Description, decimal Amount);
