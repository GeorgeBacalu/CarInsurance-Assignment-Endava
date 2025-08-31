using CarInsurance.Api.Dtos.Common;

namespace CarInsurance.Api.Dtos.Responses;
public record CarHistoryResponse(ICollection<InsurancePolicyDto> Policies, ICollection<InsuranceClaimDto> Claims);
