using CarInsurance.Api.Dtos.Common;
using CarInsurance.Api.Dtos.Requests;
using CarInsurance.Api.Dtos.Responses;
using CarInsurance.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarInsurance.Api.Controllers;

[ApiController]
[Route("api/cars")]
public class CarsController(CarService service) : ControllerBase
{
    private readonly CarService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<CarDto>>> GetCars()
        => Ok(await _service.ListCarsAsync());

    [HttpGet("{carId:long}/insurance-valid")]
    public async Task<ActionResult<InsuranceValidityResponse>> IsInsuranceValid(long carId, [FromQuery] string date)
    {
        var valid = await _service.IsInsuranceValidAsync(carId, date);
        return Ok(new InsuranceValidityResponse(carId, date, valid));
    }

    [HttpPost]
    public async Task<ActionResult<MessageResponse>> AddCar(AddCarRequest request)
    {
        await _service.AddCarAsync(request);
        return Created(string.Empty, new MessageResponse("Car added successfully"));
    }

    [HttpGet("{carId:long}/history")]
    public async Task<ActionResult<CarHistoryResponse>> GetCarHistory(long carId)
        => Ok(await _service.GetCarHistoryAsync(carId));

    [HttpPost("{carId:long}/policies")]
    public async Task<ActionResult<MessageResponse>> AddInsurancePolicy(long carId, AddInsurancePolicyRequest request)
    {
        await _service.AddInsurancePolicyAsync(carId, request);
        return Created(string.Empty, new MessageResponse("Insurance policy registered successfully"));
    }

    [HttpPost("{carId:long}/claims")]
    public async Task<ActionResult<MessageResponse>> AddInsuranceClaim(long carId, AddInsuranceClaimRequest request)
    {
        await _service.AddInsuranceClaimAsync(carId, request);
        return Created(string.Empty, new MessageResponse("Insurance claim registered successfully"));
    }
}
