using Microsoft.EntityFrameworkCore;
using WorkshopManager.Application.Abstractions;
using WorkshopManager.Application.Errors;
using WorkshopManager.Domain.Customers;

namespace WorkshopManager.Application.Customers;

public sealed class CustomerService(IAppDbContext db)
{
    public async Task<IReadOnlyList<CustomerListItemDto>> ListAsync(CancellationToken ct = default)
    {
        return await db.Customers
            .AsNoTracking()
            .Select(c => new CustomerListItemDto(
                c.Id,
                c.FullName,
                c.Phone,
                c.Email,
                c.Vehicles.Count))
            .OrderBy(x => x.FullName)
            .ToListAsync(ct);
    }

    public async Task<Customer> GetAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await db.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        return entity ?? throw new NotFoundException($"Customer '{id}' not found.");
    }

    public async Task<Customer> CreateAsync(CustomerCreateDto dto, CancellationToken ct = default)
    {
        var customer = new Customer
        {
            FullName = dto.FullName.Trim(),
            Phone = dto.Phone?.Trim(),
            Email = dto.Email?.Trim()
        };

        db.Customers.Add(customer);
        await db.SaveChangesAsync(ct);
        return customer;
    }

    public async Task<Customer> UpdateAsync(Guid id, CustomerUpdateDto dto, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException($"Customer '{id}' not found.");

        customer.FullName = dto.FullName.Trim();
        customer.Phone = dto.Phone?.Trim();
        customer.Email = dto.Email?.Trim();

        await db.SaveChangesAsync(ct);
        return customer;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var customer = await db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct)
            ?? throw new NotFoundException($"Customer '{id}' not found.");

        db.Customers.Remove(customer);
        await db.SaveChangesAsync(ct);
    }
}
