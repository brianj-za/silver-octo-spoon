using FinancialApi.Application.Interfaces;
using FinancialApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialApi.Infrastructure;

public class ReversalQuery(AccountDbContext db) : IReversalQuery
{
    public async Task<(BalancedJournalEntry?, List<Account>?)> GetReversalDataAsync(Guid journalEntryId)
    {
        var entry = await db.JournalEntries.Include(x => x.Lines)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == journalEntryId);

        if (entry == null)
        {
            return (null, null);
        }

        var accountIds = entry.Lines.Select(l => l.AccountId)
            .Distinct()
            .ToList();

        var accounts = await db.Accounts.AsNoTracking()
            .Where(a => accountIds.Contains(a.Id))
            .ToListAsync();

        return (entry, accounts);
    }
}