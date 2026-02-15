using WorkshopManager.Domain.Common;

namespace WorkshopManager.Domain.Customers;

public sealed class Customer : Entity
{
    public required string FullName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }

    public List<Vehicle> Vehicles { get; set; } = new();
}
