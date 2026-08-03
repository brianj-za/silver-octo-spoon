namespace FinancialApi.Domain.Entities;

public record JournalLine(int AccountId, decimal Amount, EntryType Type);