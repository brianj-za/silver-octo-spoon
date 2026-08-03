namespace FinancialApi.Domain.Events;

public record JournalReversedEvent(Guid OriginalJournalId, Guid ReversalJournalId);