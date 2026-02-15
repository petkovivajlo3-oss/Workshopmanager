using WorkshopManager.Domain.Common;

namespace WorkshopManager.Domain.Customers;

public sealed class Vehicle : Entity
{
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int? Year { get; set; }
    public string? Vin { get; set; }
    public string? LicensePlate { get; set; }

    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public List<ServiceOrder> ServiceOrders { get; set; } = new();
}
