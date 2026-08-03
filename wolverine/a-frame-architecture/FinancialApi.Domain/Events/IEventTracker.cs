namespace FinancialApi.Domain.Events;

public interface IEventTracker
{
    void Add(string eventMessage);
}