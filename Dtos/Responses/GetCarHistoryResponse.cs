using CarInsurance.Api.Dtos.Common;

namespace CarInsurance.Api.Dtos.Responses;
public record GetCarHistoryResponse(ICollection<InsurancePolicyDto> Policies, ICollection<InsuranceClaimDto> Claims);
