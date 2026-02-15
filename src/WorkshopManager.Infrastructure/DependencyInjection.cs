using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorkshopManager.Application.Abstractions;
using WorkshopManager.Infrastructure.Persistence;
using WorkshopManager.Infrastructure.Time;

namespace WorkshopManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default") ?? "Data Source=workshop.db";
        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(cs));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
