using FinancialApi.Application.Commands;
using FinancialApi.Application.Interfaces;
using FinancialApi.Application.Models;
using FinancialApi.Application.Strategies;
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
        FeePosting posting = new FeePosting();
        var result = posting.Apply(cmd, context);

        var @event = new FeeAppliedEvent(cmd.AccountId, cmd.Amount);

        return (Storage.Update(result.DebitAccount), Storage.Update(result.CreditAccount), Storage.Insert(result.Entry),
            @event);
    }
}