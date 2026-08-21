using FinancialApi.Application.Commands;
using FinancialApi.Application.Interfaces;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Aggregates;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Events;
using Wolverine.Persistence;

namespace FinancialApi.Application.Handlers;

public static class TransferFundsHandler
{
    public static async Task<TransferContext?> LoadAsync(
        TransferFundsCommand cmd,
        IAccountQuery query,
        TimeProvider timeProvider
    )
    {
        var source = await query.FindByIdAsync(cmd.SourceAccountId);
        var dest = await query.FindByIdAsync(cmd.DestinationAccountId);

        if (source == null || dest == null)
        {
            return null;
        }

        return new TransferContext(source, dest, timeProvider.GetUtcNow());
    }

    public static (IStorageAction<Account> SourceWrite, IStorageAction<Account> DestWrite, IStorageAction<BalancedJournalEntry>
        JournalWrite, FundsTransferredEvent Event) Handle(TransferFundsCommand cmd, TransferContext context)
    {
        var updatedSource = context.Source.ApplyPosting(cmd.Amount, EntryType.Debit);
        var updatedDest = context.Destination.ApplyPosting(cmd.Amount, EntryType.Credit);

        var lines = new List<JournalLine>
        {
            new(context.Source.Id, cmd.Amount, EntryType.Debit),
            new(context.Destination.Id, cmd.Amount, EntryType.Credit)
        };

        var journalEntry = BalancedJournal.Create(
            Guid.NewGuid(),
            $"Transfer: {cmd.Amount} from {cmd.SourceAccountId} to {cmd.DestinationAccountId}",
            context.TimeStamp,
            [.. lines]
        );

        var @event = new FundsTransferredEvent(cmd.SourceAccountId, cmd.DestinationAccountId, cmd.Amount);

        return (Storage.Update(updatedSource), Storage.Update(updatedDest), Storage.Insert(journalEntry.Entry), @event);
    }
}