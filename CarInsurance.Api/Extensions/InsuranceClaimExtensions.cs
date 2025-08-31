using CarInsurance.Api.Dtos.Requests;
using CarInsurance.Api.Models;

namespace CarInsurance.Api.Extensions;
public static class InsuranceClaimExtensions
{
    public static InsuranceClaim ToInsuranceClaim(this AddInsuranceClaimRequest request, long carId) => new InsuranceClaim
    {
        Date = request.ClaimDate,
        Description = request.Description,
        Amount = request.Amount,
        CarId = carId
    };
}
