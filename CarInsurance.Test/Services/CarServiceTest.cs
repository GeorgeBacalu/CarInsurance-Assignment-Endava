using CarInsurance.Api.Data;
using CarInsurance.Api.Exceptions;
using CarInsurance.Api.Models;
using CarInsurance.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CarInsurance.Test.Services;
public class CarServiceTest : IDisposable
{
    private const string InMemoryConnectionString = "DataSource=:memory:";

    private readonly SqliteConnection _connection;
    private readonly AppDbContext _db;
    private readonly CarService _carService;

    public CarServiceTest()
    {
        _connection = new SqliteConnection(InMemoryConnectionString);
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        _db = new AppDbContext(options);
        _db.Database.EnsureCreated();

        InitializeDatabase();

        _carService = new CarService(_db);
    }

    private void InitializeDatabase()
    {
        var ana = new Owner { Name = "Ana Pop", Email = "ana.pop@example.com" };
        var bogdan = new Owner { Name = "Bogdan Ionescu", Email = "bogdan.ionescu@example.com" };
        _db.Owners.AddRange(ana, bogdan);
        _db.SaveChanges();

        var car1 = new Car { Vin = "VIN12345", Make = "Dacia", Model = "Logan", YearOfManufacture = 2018, OwnerId = ana.Id };
        var car2 = new Car { Vin = "VIN67890", Make = "VW", Model = "Golf", YearOfManufacture = 2021, OwnerId = bogdan.Id };
        _db.Cars.AddRange(car1, car2);
        _db.SaveChanges();

        _db.Policies.AddRange(
            new InsurancePolicy { CarId = car1.Id, Provider = "Allianz", StartDate = new DateOnly(2024, 1, 1), EndDate = new DateOnly(2024, 12, 31) },
            new InsurancePolicy { CarId = car1.Id, Provider = "Groupama", StartDate = new DateOnly(2025, 1, 1), EndDate = new DateOnly(2025, 12, 31) },
            new InsurancePolicy { CarId = car2.Id, Provider = "Allianz", StartDate = new DateOnly(2025, 3, 1), EndDate = new DateOnly(2025, 9, 30) }
        );
        _db.SaveChanges();
    }

    [Fact]
    public async Task WhenDateIsAtStartBoundary_IsInsuranceValidAsync_ShouldReturnTrue()
        => Assert.True(await _carService.IsInsuranceValidAsync(1, "2024-01-01"));

    [Fact]
    public async Task WhenDateIsAtEndBoundary_IsInsuranceValidAsync_ShouldReturnTrue()
        => Assert.True(await _carService.IsInsuranceValidAsync(1, "2024-12-31"));

    [Fact]
    public async Task WhenDateIsBeforeStartBoundary_IsInsuranceValidAsync_ShouldReturnFalse()
        => Assert.False(await _carService.IsInsuranceValidAsync(1, "2023-12-31"));

    [Fact]
    public async Task WhenDateIsAfterEndBoundary_IsInsuranceValidAsync_ShouldReturnFalse()
        => Assert.False(await _carService.IsInsuranceValidAsync(1, "2026-01-01"));

    [Fact]
    public async Task WhenCarNotFound_IsInsuranceValidAsync_ShouldThrowKeyNotFoundException()
        => await Assert.ThrowsAsync<KeyNotFoundException>(() => _carService.IsInsuranceValidAsync(999, "2025-08-30"));

    [Theory]
    [InlineData("2023-02-29")]
    [InlineData("08/30/2025")]
    [InlineData("")]
    public async Task WhenDateIsInvalid_IsInsuranceValidAsync_ShouldThrowBadRequestException(string invalidDate)
        => await Assert.ThrowsAsync<BadRequestException>(() => _carService.IsInsuranceValidAsync(1, invalidDate));

    public void Dispose()
    {
        _db.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}
