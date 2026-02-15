using Microsoft.EntityFrameworkCore;
using WorkshopManager.Application.Abstractions;
using WorkshopManager.Application.Errors;
using WorkshopManager.Domain.Customers;

namespace WorkshopManager.Application.Customers;

public sealed class VehicleService(IAppDbContext db)
{
    public async Task<IReadOnlyList<VehicleListItemDto>> ListAsync(Guid? customerId, CancellationToken ct = default)
    {
        var q = db.Vehicles.AsNoTracking().AsQueryable();
        if (customerId is not null) q = q.Where(v => v.CustomerId == customerId);

        return await q
            .OrderBy(v => v.Make).ThenBy(v => v.Model)
            .Select(v => new VehicleListItemDto(
                v.Id, v.CustomerId, v.Make, v.Model, v.Year, v.Vin, v.LicensePlate
            ))
            .ToListAsync(ct);
    }

    public async Task<Vehicle> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Vehicles
            .Include(v => v.Customer)
            .Include(v => v.ServiceOrders)
            .FirstOrDefaultAsync(v => v.Id == id, ct);

        return entity ?? throw new NotFoundException($"Vehicle '{id}' not found.");
    }

    public async Task<Vehicle> CreateAsync(VehicleCreateDto dto, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == dto.CustomerId, ct)
            ?? throw new NotFoundException($"Customer '{dto.CustomerId}' not found.");

        var vehicle = new Vehicle
        {
            CustomerId = customer.Id,
            Make = dto.Make.Trim(),
            Model = dto.Model.Trim(),
            Year = dto.Year,
            Vin = dto.Vin?.Trim(),
            LicensePlate = dto.LicensePlate?.Trim()
        };

        db.Vehicles.Add(vehicle);
        await db.SaveChangesAsync(ct);
        return vehicle;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var vehicle = await db.Vehicles.FirstOrDefaultAsync(v => v.Id == id, ct)
            ?? throw new NotFoundException($"Vehicle '{id}' not found.");

        db.Vehicles.Remove(vehicle);
        await db.SaveChangesAsync(ct);
    }
}
