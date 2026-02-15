namespace WorkshopManager.Application.Customers;

public sealed record VehicleCreateDto(
    Guid CustomerId,
    string Make,
    string Model,
    int? Year,
    string? Vin,
    string? LicensePlate);

public sealed record VehicleListItemDto(
    Guid Id,
    Guid CustomerId,
    string Make,
    string Model,
    int? Year,
    string? Vin,
    string? LicensePlate);
