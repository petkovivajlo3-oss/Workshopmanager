using Microsoft.EntityFrameworkCore;
using WorkshopManager.Domain.Customers;
using WorkshopManager.Domain.Service;

namespace WorkshopManager.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Customers.AnyAsync(ct)) return;

        var ivan = new Customer { FullName = "Иван Георгиев", Phone = "+359 88 123 4567", Email = "ivan@example.com" };
        var maria = new Customer { FullName = "Мария Петрова", Phone = "+359 89 222 3333", Email = "maria@example.com" };

        var car1 = new Vehicle { CustomerId = ivan.Id, Make = "Volkswagen", Model = "Golf 6", Year = 2009, LicensePlate = "CA1234AB" };
        var car2 = new Vehicle { CustomerId = maria.Id, Make = "Toyota", Model = "Corolla", Year = 2016, LicensePlate = "PB5678CD" };

        var order = new ServiceOrder
        {
            VehicleId = car1.Id,
            Status = ServiceOrderStatus.Scheduled,
            ScheduledForUtc = DateTime.UtcNow.AddDays(2),
            CustomerComplaint = "Странен шум отпред при неравности.",
            LaborCost = 60m
        };
        order.Lines.Add(new ServiceOrderLine { ServiceOrderId = order.Id, Description = "Тампон стабилизираща щанга", Quantity = 2, UnitPrice = 18m });
        order.PartsCost = order.Lines.Sum(l => l.LineTotal);

        db.Customers.AddRange(ivan, maria);
        db.Vehicles.AddRange(car1, car2);
        db.ServiceOrders.Add(order);

        await db.SaveChangesAsync(ct);
    }
}
