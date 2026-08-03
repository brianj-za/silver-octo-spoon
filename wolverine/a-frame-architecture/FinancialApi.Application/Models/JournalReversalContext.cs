using FinancialApi.Domain.Entities;

namespace FinancialApi.Application.Models;

public record JournalReversalContext(
    BalancedJournalEntry OriginalBalancedJournalEntry,
    IReadOnlyList<Account> AffectedAccounts,
    DateTimeOffset TimeStamp
);