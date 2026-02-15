using Microsoft.EntityFrameworkCore;
using WorkshopManager.Application.Abstractions;
using WorkshopManager.Application.Errors;
using WorkshopManager.Domain.Service;

namespace WorkshopManager.Application.Service;

public sealed class ServiceOrderService(IAppDbContext db, IClock clock)
{
    public async Task<IReadOnlyList<object>> ListAsync(Guid? vehicleId, CancellationToken ct = default)
    {
        var q = db.ServiceOrders.AsNoTracking().AsQueryable();
        if (vehicleId is not null) q = q.Where(o => o.VehicleId == vehicleId);

        return await q
            .OrderByDescending(o => o.CreatedAtUtc)
            .Select(o => new
            {
                o.Id,
                o.VehicleId,
                o.Status,
                o.ScheduledForUtc,
                o.CustomerComplaint,
                o.LaborCost,
                o.PartsCost,
                Total = o.Total
            })
            .Cast<object>()
            .ToListAsync(ct);
    }

    public async Task<ServiceOrder> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.ServiceOrders
            .Include(o => o.Vehicle)
            .ThenInclude(v => v!.Customer)
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

        return entity ?? throw new NotFoundException($"Service order '{id}' not found.");
    }

    public async Task<ServiceOrder> CreateAsync(ServiceOrderCreateDto dto, CancellationToken ct = default)
    {
        _ = await db.Vehicles.FirstOrDefaultAsync(v => v.Id == dto.VehicleId, ct)
            ?? throw new NotFoundException($"Vehicle '{dto.VehicleId}' not found.");

        var order = new ServiceOrder
        {
            VehicleId = dto.VehicleId,
            ScheduledForUtc = dto.ScheduledForUtc,
            Status = dto.ScheduledForUtc is null ? ServiceOrderStatus.Draft : ServiceOrderStatus.Scheduled,
            CustomerComplaint = dto.CustomerComplaint?.Trim(),
            LaborCost = 0,
            PartsCost = 0
        };

        db.ServiceOrders.Add(order);
        await db.SaveChangesAsync(ct);
        return order;
    }

    public async Task<ServiceOrder> UpdateAsync(Guid id, ServiceOrderUpdateDto dto, CancellationToken ct = default)
    {
        var order = await db.ServiceOrders.FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new NotFoundException($"Service order '{id}' not found.");

        order.ScheduledForUtc = dto.ScheduledForUtc;
        order.Status = dto.Status;
        order.TechnicianNotes = dto.TechnicianNotes?.Trim();
        order.LaborCost = dto.LaborCost;
        order.PartsCost = dto.PartsCost;

        await db.SaveChangesAsync(ct);
        return order;
    }

    public async Task<ServiceOrder> AddLineAsync(Guid orderId, ServiceOrderLineAddDto dto, CancellationToken ct = default)
    {
        var order = await db.ServiceOrders
            .Include(o => o.Lines)
            .FirstOrDefaultAsync(o => o.Id == orderId, ct)
            ?? throw new NotFoundException($"Service order '{orderId}' not found.");

        order.Lines.Add(new ServiceOrderLine
        {
            ServiceOrderId = order.Id,
            Description = dto.Description.Trim(),
            Quantity = Math.Max(1, dto.Quantity),
            UnitPrice = dto.UnitPrice
        });

        // keep PartsCost in sync with lines total (simple approach)
        order.PartsCost = order.Lines.Sum(l => l.LineTotal);

        await db.SaveChangesAsync(ct);
        return order;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var order = await db.ServiceOrders.FirstOrDefaultAsync(o => o.Id == id, ct)
            ?? throw new NotFoundException($"Service order '{id}' not found.");

        db.ServiceOrders.Remove(order);
        await db.SaveChangesAsync(ct);
    }
}
