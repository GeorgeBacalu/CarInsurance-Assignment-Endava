namespace CarInsurance.Api.Dtos.Common;
public record CarDto(long Id, string Vin, string? Make, string? Model, int Year, long OwnerId, string OwnerName, string? OwnerEmail);
