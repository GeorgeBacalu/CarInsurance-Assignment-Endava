namespace CarInsurance.Api.Dtos.Requests;
public record AddCarRequest(long Id, string Vin, string? Make, string? Model, int Year, long OwnerId);
