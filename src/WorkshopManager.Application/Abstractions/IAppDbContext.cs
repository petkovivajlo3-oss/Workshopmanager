using Microsoft.EntityFrameworkCore;
using WorkshopManager.Domain.Customers;
using WorkshopManager.Domain.Service;

namespace WorkshopManager.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Customer> Customers { get; }
    DbSet<Vehicle> Vehicles { get; }
    DbSet<ServiceOrder> ServiceOrders { get; }
    DbSet<ServiceOrderLine> ServiceOrderLines { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
