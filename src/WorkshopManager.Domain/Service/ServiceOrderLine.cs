using WorkshopManager.Domain.Common;

namespace WorkshopManager.Domain.Service;

public sealed class ServiceOrderLine : Entity
{
    public Guid ServiceOrderId { get; set; }
    public ServiceOrder? ServiceOrder { get; set; }

    public required string Description { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;
}
