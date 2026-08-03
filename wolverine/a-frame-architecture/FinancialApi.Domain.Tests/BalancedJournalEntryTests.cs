using FinancialApi.Domain.Aggregates;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;

namespace FinancialApi.Domain.Tests;

public class BalancedJournalEntryTests
{
    [Fact]
    public void GivenBalanceJournal_ShouldSucceed()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        List<JournalLine> lines = [new(1, 100, EntryType.Debit), new(1, 100, EntryType.Credit)];

        // Act
        var act = () => BalancedJournal.Create(Guid.NewGuid(), "Transfer", timeProvider.GetUtcNow(), lines);

        // Assert
        act.Should()
            .NotThrow();
    }

    [Fact]
    public void GivenUnbalancedJournal_ShouldFail()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        List<JournalLine> lines = [new(1, 10, EntryType.Debit), new(1, 100, EntryType.Credit)];

        // Act
        var act = () => BalancedJournal.Create(Guid.NewGuid(), "Transfer", timeProvider.GetUtcNow(), lines);

        // Assert
        act.Should()
            .Throw<GaapViolationException>();
    }

    [Fact]
    public void GivenEmptyJournal_ShouldFail()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();

        // Act
        var act = () => BalancedJournal.Create(Guid.NewGuid(), "Transfer", timeProvider.GetUtcNow(), []);

        // Assert
        act.Should()
            .Throw<GaapViolationException>();
    }
}