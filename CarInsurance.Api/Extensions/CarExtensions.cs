using CarInsurance.Api.Dtos.Requests;
using CarInsurance.Api.Models;

namespace CarInsurance.Api.Extensions;
public static class CarExtensions
{
    public static Car ToCar(this AddCarRequest request) => new Car
    {
        Id = request.Id,
        Vin = request.Vin,
        Make = request.Make,
        Model = request.Model,
        YearOfManufacture = request.Year,
        OwnerId = request.OwnerId
    };
}
