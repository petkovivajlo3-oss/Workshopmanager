using WorkshopManager.Application.Abstractions;

namespace WorkshopManager.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
