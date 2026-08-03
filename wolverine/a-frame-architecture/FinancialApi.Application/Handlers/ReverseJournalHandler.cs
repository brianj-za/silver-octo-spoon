using FinancialApi.Application.Commands;
using FinancialApi.Application.Interfaces;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Aggregates;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Events;
using Wolverine.Persistence;

namespace FinancialApi.Application.Handlers;

public static class ReverseJournalHandler
{
    public static async Task<JournalReversalContext?> LoadAsync(
        ReverseJournalCommand cmd,
        IReversalQuery query,
        TimeProvider timeProvider
    )
    {
        var (journalEnty, accounts) = await query.GetReversalDataAsync(cmd.OriginalJournalId);
        if (journalEnty == null)
        {
            throw new InvalidOperationException("Journal not found");
        }

        if (accounts == null)
        {
            throw new InvalidOperationException("Journal accounts not found");
        }

        return new JournalReversalContext(journalEnty, accounts, timeProvider.GetUtcNow()) ??
               throw new InvalidOperationException($"Journal Entry {cmd.OriginalJournalId} does not exist");
    }

    public static (IStorageAction<BalancedJournalEntry> JournalWrite, UnitOfWork<Account> AccountWrites, JournalReversedEvent
        Event) Handle(ReverseJournalCommand cmd, JournalReversalContext context)
    {
        var reversalJournal = BalancedJournal.CreateReversal(
            context.OriginalBalancedJournalEntry,
            $"Reversal: {cmd.Reason}",
            context.TimeStamp
        );
        var accountState = context.AffectedAccounts.ToDictionary(a => a.Id);

        reversalJournal.Entry.Lines.ForEach(l =>
            {
                var currentAccount = accountState[l.AccountId];
                accountState[l.AccountId] = currentAccount.ApplyPosting(l.Amount, l.Type);
            }
        );
        var accountUow = new UnitOfWork<Account>();
        foreach (var account in accountState.Values)
        {
            accountUow.Update(account);
        }

        return (Storage.Insert(reversalJournal.Entry), accountUow,
            new JournalReversedEvent(cmd.OriginalJournalId, reversalJournal.Entry.Id));
    }
}