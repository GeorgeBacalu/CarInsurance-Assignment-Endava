using CarInsurance.Api.Data;
using CarInsurance.Api.Dtos.Common;
using CarInsurance.Api.Dtos.Requests;
using CarInsurance.Api.Dtos.Responses;
using CarInsurance.Api.Exceptions;
using CarInsurance.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Api.Services;

public class CarService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<CarDto>> ListCarsAsync()
    {
        return await _db.Cars.Include(c => c.Owner)
            .Select(c => new CarDto(c.Id, c.Vin, c.Make, c.Model, c.YearOfManufacture, c.OwnerId, c.Owner.Name, c.Owner.Email))
            .ToListAsync();
    }

    public async Task<bool> IsInsuranceValidAsync(long carId, string date)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            throw new BadRequestException("Invalid date format. It should be formatted as 'YYYY-MM-DD'.");

        var carExists = await _db.Cars.AnyAsync(c => c.Id == carId);
        if (!carExists) throw new KeyNotFoundException($"Car {carId} not found");

        return await _db.Policies.AnyAsync(p => p.CarId == carId && p.StartDate <= parsedDate && p.EndDate >= parsedDate);
    }

    public async Task AddCarAsync(AddCarRequest request)
    {
        var car = request.ToCar();

        await _db.Cars.AddAsync(car);
        await _db.SaveChangesAsync();
    }

    public async Task<GetCarHistoryResponse> GetCarHistoryAsync(long carId)
    {
        var policies = await _db.Policies.Where(p => p.CarId == carId)
            .OrderBy(p => p.StartDate)
            .Select(p => new InsurancePolicyDto(p.Id, p.Provider, p.StartDate, p.EndDate))
            .ToListAsync();
        var claims = await _db.Claims.Where(c => c.CarId == carId)
            .OrderBy(c => c.Date)
            .Select(c => new InsuranceClaimDto(c.Id, c.Date, c.Description, c.Amount))
            .ToListAsync();

        return new GetCarHistoryResponse(policies, claims);
    }

    public async Task AddInsuranceClaimAsync(long carId, AddInsuranceClaimRequest request)
    {
        var insuranceClaim = request.ToInsuranceClaim(carId);

        await _db.AddAsync(insuranceClaim);
        await _db.SaveChangesAsync();
    }
}
