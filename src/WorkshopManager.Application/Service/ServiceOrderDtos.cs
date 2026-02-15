using WorkshopManager.Domain.Service;

namespace WorkshopManager.Application.Service;

public sealed record ServiceOrderCreateDto(
    Guid VehicleId,
    DateTime? ScheduledForUtc,
    string? CustomerComplaint);

public sealed record ServiceOrderUpdateDto(
    DateTime? ScheduledForUtc,
    ServiceOrderStatus Status,
    string? TechnicianNotes,
    decimal LaborCost,
    decimal PartsCost);

public sealed record ServiceOrderLineAddDto(
    string Description,
    int Quantity,
    decimal UnitPrice);
