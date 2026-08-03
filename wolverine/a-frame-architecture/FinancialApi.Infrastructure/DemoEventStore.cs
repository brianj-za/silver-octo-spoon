using FinancialApi.Domain.Events;

namespace FinancialApi.Infrastructure;

public class DemoEventStore : IEventTracker
{
    public List<string> CapturedEvents { get; } = new();

    public void Add(string eventMessage)
    {
        CapturedEvents.Add($"[{DateTimeOffset.UtcNow:HH:mm:ss}] {eventMessage}");
    }
}