using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Application.Customers;
using WorkshopManager.Application.Errors;
using WorkshopManager.Application.Service;
using WorkshopManager.Infrastructure;
using WorkshopManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<VehicleService>();
builder.Services.AddScoped<ServiceOrderService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(exceptionApp =>
{
    exceptionApp.Run(async context =>
    {
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (ex is null) return;

        context.Response.ContentType = "application/json";

        var (status, title) = ex switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad request"),
            _ => (StatusCodes.Status500InternalServerError, "Server error")
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = ex.Message
        });
    });
});

// DB migrate + seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(db);
}

// Health
app.MapGet("/", () => Results.Ok(new
{
    name = "WorkshopManager API",
    utc = DateTime.UtcNow,
    docs = "/swagger"
}));

// Customers
app.MapGet("/api/customers", async ([FromServices] CustomerService svc, CancellationToken ct) =>
    Results.Ok(await svc.ListAsync(ct)));

app.MapGet("/api/customers/{id:guid}", async ([FromServices] CustomerService svc, Guid id, CancellationToken ct) =>
    Results.Ok(await svc.GetAsync(id, ct)));

app.MapPost("/api/customers", async ([FromServices] CustomerService svc, [FromBody] CustomerCreateDto dto, CancellationToken ct) =>
    Results.Created($"/api/customers", await svc.CreateAsync(dto, ct)));

app.MapPut("/api/customers/{id:guid}", async ([FromServices] CustomerService svc, Guid id, [FromBody] CustomerUpdateDto dto, CancellationToken ct) =>
    Results.Ok(await svc.UpdateAsync(id, dto, ct)));

app.MapDelete("/api/customers/{id:guid}", async ([FromServices] CustomerService svc, Guid id, CancellationToken ct) =>
{
    await svc.DeleteAsync(id, ct);
    return Results.NoContent();
});

// Vehicles
app.MapGet("/api/vehicles", async ([FromServices] VehicleService svc, Guid? customerId, CancellationToken ct) =>
    Results.Ok(await svc.ListAsync(customerId, ct)));

app.MapGet("/api/vehicles/{id:guid}", async ([FromServices] VehicleService svc, Guid id, CancellationToken ct) =>
    Results.Ok(await svc.GetAsync(id, ct)));

app.MapPost("/api/vehicles", async ([FromServices] VehicleService svc, [FromBody] VehicleCreateDto dto, CancellationToken ct) =>
    Results.Created($"/api/vehicles", await svc.CreateAsync(dto, ct)));

app.MapDelete("/api/vehicles/{id:guid}", async ([FromServices] VehicleService svc, Guid id, CancellationToken ct) =>
{
    await svc.DeleteAsync(id, ct);
    return Results.NoContent();
});

// Service Orders
app.MapGet("/api/orders", async ([FromServices] ServiceOrderService svc, Guid? vehicleId, CancellationToken ct) =>
    Results.Ok(await svc.ListAsync(vehicleId, ct)));

app.MapGet("/api/orders/{id:guid}", async ([FromServices] ServiceOrderService svc, Guid id, CancellationToken ct) =>
    Results.Ok(await svc.GetAsync(id, ct)));

app.MapPost("/api/orders", async ([FromServices] ServiceOrderService svc, [FromBody] ServiceOrderCreateDto dto, CancellationToken ct) =>
    Results.Created($"/api/orders", await svc.CreateAsync(dto, ct)));

app.MapPut("/api/orders/{id:guid}", async ([FromServices] ServiceOrderService svc, Guid id, [FromBody] ServiceOrderUpdateDto dto, CancellationToken ct) =>
    Results.Ok(await svc.UpdateAsync(id, dto, ct)));

app.MapPost("/api/orders/{id:guid}/lines", async ([FromServices] ServiceOrderService svc, Guid id, [FromBody] ServiceOrderLineAddDto dto, CancellationToken ct) =>
    Results.Ok(await svc.AddLineAsync(id, dto, ct)));

app.MapDelete("/api/orders/{id:guid}", async ([FromServices] ServiceOrderService svc, Guid id, CancellationToken ct) =>
{
    await svc.DeleteAsync(id, ct);
    return Results.NoContent();
});

app.Run();
