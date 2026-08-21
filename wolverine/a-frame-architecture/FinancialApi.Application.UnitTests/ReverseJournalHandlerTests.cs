using FinancialApi.Application.Commands;
using FinancialApi.Application.Handlers;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Events;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.Extensions.Time.Testing;
using Wolverine.Persistence;

namespace FinancialApi.Application.UnitTests;

public class ReverseJournalHandlerTests
{
    [Fact]
    public void GivenValidJournalEntry_Reversal_ShouldResetAccountBalances()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        var expectedSourceAccountBalance = 100.0m;
        var expectedDestAccountBalance = 100.0m;
        var (initialSourceAccountWrite, initialDestAccountWrite, initialJournalWrite, initialEvent) =
            CreateInitialTransferTransaction(
                expectedSourceAccountBalance,
                expectedDestAccountBalance,
                10,
                timeProvider
            );
        var command = new ReverseJournalCommand(initialJournalWrite.Entity.Id, "Invalid Transaction");
        var context = new JournalReversalContext(
            initialJournalWrite.Entity,
            [initialSourceAccountWrite.Entity, initialDestAccountWrite.Entity],
            timeProvider.GetUtcNow()
        );

        // Act
        var (journalWrite, accountWrites, @event) = ReverseJournalHandler.Handle(command, context);

        // Assert
        using var _ = new AssertionScope();
        accountWrites[0]
            .Entity.Balance.Should()
            .Be(expectedSourceAccountBalance);
        accountWrites[1]
            .Entity.Balance.Should()
            .Be(expectedDestAccountBalance);
        journalWrite.Entity.Lines.Should()
            .HaveCount(initialJournalWrite.Entity.Lines.Length);
    }

    private (IStorageAction<Account> SourceWrite, IStorageAction<Account> DestWrite, IStorageAction<BalancedJournalEntry>
        JournalWrite, FundsTransferredEvent Event) CreateInitialTransferTransaction(
            decimal sourceStartingBalance,
            decimal destStartingBalance,
            decimal transferAmount,
            TimeProvider timeProvider
        )
    {
        var command = new TransferFundsCommand(1, 2, transferAmount);
        var sourceAccount = new Account(1, sourceStartingBalance, AccountType.Spend);
        var destAccount = new Account(2, destStartingBalance, AccountType.Spend);
        var transferContext = new TransferContext(sourceAccount, destAccount, timeProvider.GetUtcNow());

        // Act
        return TransferFundsHandler.Handle(command, transferContext);
    }
}