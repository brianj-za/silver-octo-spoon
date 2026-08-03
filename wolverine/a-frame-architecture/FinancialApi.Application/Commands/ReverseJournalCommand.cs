namespace FinancialApi.Application.Commands;

public record ReverseJournalCommand(Guid OriginalJournalId, string Reason);