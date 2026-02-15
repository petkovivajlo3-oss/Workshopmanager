namespace WorkshopManager.Application.Customers;

public sealed record CustomerCreateDto(string FullName, string? Phone, string? Email);
public sealed record CustomerUpdateDto(string FullName, string? Phone, string? Email);

public sealed record CustomerListItemDto(
    Guid Id,
    string FullName,
    string? Phone,
    string? Email,
    int VehiclesCount);
