using FinancialApi.Domain.Entities;

namespace FinancialApi.Application.Interfaces;

public interface IReversalQuery
{
    Task<(BalancedJournalEntry?, List<Account>?)> GetReversalDataAsync(Guid journalEntryId);
}