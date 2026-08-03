using FinancialApi.Domain.Events;
using Microsoft.Extensions.Logging;

namespace FinancialApi.Application.Handlers;

public static class FinancialEventHandler
{
    public static void Handle(FeeAppliedEvent @event, IEventTracker store, ILogger logger)
    {
        var message = $"FeeApplied: ${@event.Amount} moved from Account {@event.AccountId} to Account 99999.";
        logger.LogInformation("BACKGROUND EVENT FIRED: {Message}", message);
        store.Add(message);
    }

    public static void Handle(FundsTransferredEvent @event, IEventTracker store, ILogger logger)
    {
        var message =
            $"FundsTransferred: ${@event.Amount} moved from Account {@event.SourceAccountId} to Account {@event.DestinationAccountId}.";
        logger.LogInformation("BACKGROUND EVENT FIRED: {Message}", message);
        store.Add(message);
    }

    public static void Handle(JournalReversedEvent @event, IEventTracker store, ILogger logger)
    {
        var message =
            $"JournalReversed: Original Entry {@event.OriginalJournalId} was reversed by Entry {@event.ReversalJournalId}.";
        logger.LogInformation("BACKGROUND EVENT FIRED: {Message}", message);
        store.Add(message);
    }
}