using System.Diagnostics;
using FinancialApi.Domain.Aggregates;
using FinancialApi.Domain.Entities;

namespace FinancialApi.Domain.Services;

public sealed record PostingResult(Account DebitAccount, Account CreditAccount, BalancedJournalEntry Entry);

public static class Ledger
{
    public static PostingResult
        PostBalanced(decimal amount, string memo, DateTimeOffset when, Account debit, Account credit) => new(
        debit.ApplyPosting(amount, EntryType.Debit),
        credit.ApplyPosting(amount, EntryType.Credit),
        BalancedJournal.Create(
                Guid.NewGuid(),
                memo,
                when,
                [
                    new JournalLine(debit.Id, amount, EntryType.Debit),
                    new JournalLine(credit.Id, amount, EntryType.Credit)
                ]
            )
            .Entry
    );
}