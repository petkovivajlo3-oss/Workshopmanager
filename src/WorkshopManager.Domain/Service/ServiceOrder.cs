using WorkshopManager.Domain.Common;
using WorkshopManager.Domain.Customers;

namespace WorkshopManager.Domain.Service;

public enum ServiceOrderStatus
{
    Draft = 0,
    Scheduled = 1,
    InProgress = 2,
    Done = 3,
    Cancelled = 4
}

public sealed class ServiceOrder : Entity
{
    public Guid VehicleId { get; set; }
    public Vehicle? Vehicle { get; set; }

    public DateTime? ScheduledForUtc { get; set; }
    public ServiceOrderStatus Status { get; set; } = ServiceOrderStatus.Draft;

    public string? CustomerComplaint { get; set; }
    public string? TechnicianNotes { get; set; }

    public decimal LaborCost { get; set; }
    public decimal PartsCost { get; set; }

    public List<ServiceOrderLine> Lines { get; set; } = new();

    public decimal Total => LaborCost + PartsCost;
}
