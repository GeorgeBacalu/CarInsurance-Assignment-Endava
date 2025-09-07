namespace CarInsurance.Api.Dtos.Common;
public record InsurancePolicyDto(long Id, string? Provider, DateOnly StartDate, DateOnly EndDate);
