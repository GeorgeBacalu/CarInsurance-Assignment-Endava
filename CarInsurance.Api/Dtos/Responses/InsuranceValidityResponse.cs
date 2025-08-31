namespace CarInsurance.Api.Dtos.Responses;
public record InsuranceValidityResponse(long CarId, string Date, bool Valid);
