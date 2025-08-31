using CarInsurance.Api.Dtos.Requests;
using CarInsurance.Api.Models;

namespace CarInsurance.Api.Extensions;
public static class InsurancePolicyExtensions
{
    public static InsurancePolicy ToInsurancePolicy(this AddInsurancePolicyRequest request, long carId) => new InsurancePolicy
    {
        Provider = request.Provider,
        StartDate = request.StartDate,
        EndDate = request.EndDate,
        CarId = carId
    };
}
