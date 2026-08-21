using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Exceptions;

namespace FinancialApi.Domain.Aggregates;

public record BalancedJournal
{
    public BalancedJournalEntry Entry { get; }

    private BalancedJournal(BalancedJournalEntry entry)
    {
        Entry = entry;
    }

    public static BalancedJournal Create(Guid id, string description, DateTimeOffset createdAt, JournalLine[] lines)
    {
        if (lines.Count(l => l.Type == EntryType.Debit) == 0)
        {
            throw new GaapViolationException($"GAAP Violation: A journal entry needs at least one debit line.");
        }

        if (lines.Count(l => l.Type == EntryType.Credit) == 0)
        {
            throw new GaapViolationException($"A journal entry needs at least one credit line.");
        }

        var debits = lines.Where(l => l.Type == EntryType.Debit)
            .Sum(l => l.Amount);
        var credits = lines.Where(l => l.Type == EntryType.Credit)
            .Sum(l => l.Amount);

        return debits != credits
            ? throw new GaapViolationException($"Total Debits ({debits}) must equal Total Credits ({credits})!")
            : new BalancedJournal(new BalancedJournalEntry(id, description, createdAt, lines));
    }

    public static BalancedJournal CreateReversal(BalancedJournalEntry original, string reason, DateTimeOffset now)
    {
        var reversedLines = original.Lines.Select(l =>
                l with { Type = l.Type == EntryType.Debit ? EntryType.Credit : EntryType.Debit }
            )
            .ToArray();

        return Create(Guid.NewGuid(), reason, now, reversedLines);
    }
}