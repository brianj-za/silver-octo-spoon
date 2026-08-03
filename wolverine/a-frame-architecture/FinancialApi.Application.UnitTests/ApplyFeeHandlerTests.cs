using FinancialApi.Application.Commands;
using FinancialApi.Application.Handlers;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Time.Testing;

namespace FinancialApi.Application.UnitTests;

public class ApplyFeeHandlerTests
{
    public class SpendAccount
    {
        [Theory]
        [InlineData(100.0, 20.0, 80.0)]
        [InlineData(199.78, 10.0, 189.78)]
        [InlineData(200000, 0.09, 199999.91)]
        [InlineData(1000, 1000, 0)]
        public void GivenStartingBalanceGreaterThanFee_ApplyFee_ShouldBeExpectedBalance(
            decimal startingBalance,
            decimal feeAmount,
            decimal expectedBalance
        )
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new ApplyFeeCommand(1, feeAmount);
            var account = new Account(1, startingBalance, AccountType.Spend);
            var revenueAccount = new Account(99999, 0, AccountType.Revenue);
            var feeContext = new FeeContext(account, revenueAccount, timeProvider.GetUtcNow());

            // Act
            var (customerWrite, revenueWrite, journalWrite, @event) = ApplyFeeHandler.Handle(command, feeContext);

            // Assert
            using var _ = new AssertionScope();
            customerWrite.Entity.Balance.Should()
                .Be(expectedBalance);
        }

        [Theory]
        [InlineData(100.0, 110.0)]
        [InlineData(199.78, 200.0)]
        [InlineData(200000, 200000.01)]
        [InlineData(1000, 1999999)]
        public void GivenStartingBalanceLessThanFee_ApplyFee_ShouldFail(decimal startingBalance, decimal feeAmount)
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new ApplyFeeCommand(1, feeAmount);
            var account = new Account(1, startingBalance, AccountType.Spend);
            var revenueAccount = new Account(99999, 0, AccountType.Revenue);
            var feeContext = new FeeContext(account, revenueAccount, timeProvider.GetUtcNow());

            // Act
            var act = () => ApplyFeeHandler.Handle(command, feeContext);

            // Assert
            using var _ = new AssertionScope();
            act.Should()
                .Throw<InsufficientFundsException>();
        }
    }

    public class BorrowAccount
    {
        [Theory]
        [InlineData(100.0, 20.0, 80.0)]
        [InlineData(199.78, 10.0, 189.78)]
        [InlineData(200000, 0.09, 199999.91)]
        [InlineData(1000, 1000, 0)]
        public void GivenStartingBalanceGreaterThanFee_ApplyFee_ShouldBeExpectedBalance(
            decimal startingBalance,
            decimal feeAmount,
            decimal expectedBalance
        )
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new ApplyFeeCommand(1, feeAmount);
            var account = new Account(1, startingBalance, AccountType.Borrow);
            var revenueAccount = new Account(99999, 0, AccountType.Revenue);
            var feeContext = new FeeContext(account, revenueAccount, timeProvider.GetUtcNow());

            // Act
            var (customerWrite, revenueWrite, journalWrite, @event) = ApplyFeeHandler.Handle(command, feeContext);

            // Assert
            using var _ = new AssertionScope();
            customerWrite.Entity.Balance.Should()
                .Be(expectedBalance);
        }

        [Theory]
        [InlineData(100.0, 110.0, -10.0)]
        [InlineData(199.78, 200.0, -0.22)]
        [InlineData(200000, 200000.01, -0.01)]
        [InlineData(1000, 1999999, -1998999)]
        public void GivenStartingBalanceLessThanFee_ApplyFee_ShouldBeExpectedBalance(
            decimal startingBalance,
            decimal feeAmount,
            decimal expectedBalance
        )
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new ApplyFeeCommand(1, feeAmount);
            var account = new Account(1, startingBalance, AccountType.Borrow);
            var revenueAccount = new Account(99999, 0, AccountType.Revenue);
            var feeContext = new FeeContext(account, revenueAccount, timeProvider.GetUtcNow());

            // Act
            var (customerWrite, revenueWrite, journalWrite, @event) = ApplyFeeHandler.Handle(command, feeContext);

            // Assert
            using var _ = new AssertionScope();
            customerWrite.Entity.Balance.Should()
                .Be(expectedBalance);
            revenueWrite.Entity.Balance.Should()
                .Be(feeAmount);
            journalWrite.Should()
                .NotBeNull();
            @event.Should()
                .NotBeNull();
        }
    }
}