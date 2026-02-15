using Microsoft.EntityFrameworkCore;
using WorkshopManager.Application.Abstractions;
using WorkshopManager.Domain.Customers;
using WorkshopManager.Domain.Service;

namespace WorkshopManager.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ServiceOrder> ServiceOrders => Set<ServiceOrder>();
    public DbSet<ServiceOrderLine> ServiceOrderLines => Set<ServiceOrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            b.Property(x => x.Phone).HasMaxLength(50);
            b.Property(x => x.Email).HasMaxLength(200);

            b.HasMany(x => x.Vehicles)
                .WithOne(x => x.Customer)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Vehicle>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Make).HasMaxLength(100).IsRequired();
            b.Property(x => x.Model).HasMaxLength(100).IsRequired();
            b.Property(x => x.Vin).HasMaxLength(50);
            b.Property(x => x.LicensePlate).HasMaxLength(20);

            b.HasMany(x => x.ServiceOrders)
                .WithOne(x => x.Vehicle)
                .HasForeignKey(x => x.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceOrder>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Status).IsRequired();
            b.Property(x => x.CustomerComplaint).HasMaxLength(2000);
            b.Property(x => x.TechnicianNotes).HasMaxLength(4000);
            b.Property(x => x.LaborCost).HasPrecision(18, 2);
            b.Property(x => x.PartsCost).HasPrecision(18, 2);

            b.Ignore(x => x.Total);

            b.HasMany(x => x.Lines)
                .WithOne(x => x.ServiceOrder)
                .HasForeignKey(x => x.ServiceOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceOrderLine>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Description).HasMaxLength(500).IsRequired();
            b.Property(x => x.UnitPrice).HasPrecision(18, 2);
            b.Ignore(x => x.LineTotal);
        });
    }
}
