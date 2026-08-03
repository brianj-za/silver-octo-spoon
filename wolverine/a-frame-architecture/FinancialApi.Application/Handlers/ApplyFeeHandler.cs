using FinancialApi.Application.Commands;
using FinancialApi.Application.Interfaces;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Aggregates;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Events;
using Wolverine.Persistence;

namespace FinancialApi.Application.Handlers;

public static class ApplyFeeHandler
{
    public static async Task<FeeContext> LoadAsync(ApplyFeeCommand cmd, IAccountQuery query, TimeProvider timeProvider)
    {
        var source = await query.FindByIdAsync(cmd.AccountId);
        var dest = await query.FindByIdAsync(99999);
        if (source == null || dest == null)
        {
            throw new InvalidOperationException("Cannot process transfer. Account(s) not found.");
        }

        return new FeeContext(source, dest, timeProvider.GetUtcNow());
    }

    public static ( IStorageAction<Account> CustomerWrite, IStorageAction<Account> RevenueWrite,
        IStorageAction<BalancedJournalEntry> JournalWrite, FeeAppliedEvent Event ) Handle(
            ApplyFeeCommand cmd,
            FeeContext context
        )
    {
        var updatedSource = context.Source.ApplyPosting(cmd.Amount, EntryType.Debit);
        var updatedRevenue = context.Destination.ApplyPosting(cmd.Amount, EntryType.Credit);

        var lines = new List<JournalLine>
        {
            new(updatedSource.Id, cmd.Amount, EntryType.Debit), new(updatedRevenue.Id, cmd.Amount, EntryType.Credit)
        };

        var journalEntry = BalancedJournal.Create(
            Guid.NewGuid(),
            $"Service Fee Applied: {cmd.Amount} to Account {cmd.AccountId}",
            context.TimeStamp,
            lines
        );

        var @event = new FeeAppliedEvent(cmd.AccountId, cmd.Amount);

        return (Storage.Update(updatedSource), Storage.Update(updatedRevenue), Storage.Insert(journalEntry.Entry),
            @event);
    }
}