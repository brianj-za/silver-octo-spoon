using FinancialApi.Application.Commands;
using FinancialApi.Application.Handlers;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Exceptions;
using AwesomeAssertions;
using AwesomeAssertions.Execution;
using Microsoft.Extensions.Time.Testing;

namespace FinancialApi.Application.UnitTests;

public class TransferFundsHandlerTests
{
    public class LiabilityAccount
    {
        [Theory]
        [InlineData(100.0, 20.0, 80.0)]
        [InlineData(199.78, 10.0, 189.78)]
        [InlineData(200000, 0.09, 199999.91)]
        [InlineData(1000, 1000, 0)]
        public void GivenStartingBalanceGreaterThanTransfer_ApplyFee_ShouldBeExpectedBalance(
            decimal startingBalance,
            decimal feeAmount,
            decimal expectedBalance
        )
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new TransferFundsCommand(1, 2, feeAmount);
            var sourceAccount = new Account(1, startingBalance, AccountType.Spend);
            var destAccount = new Account(2, 0, AccountType.Spend);
            var transferContext = new TransferContext(sourceAccount, destAccount, timeProvider.GetUtcNow());

            // Act
            var (customerWrite, revenueWrite, journalWrite, @event) =
                TransferFundsHandler.Handle(command, transferContext);

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
        public void GivenStartingBalanceLessThanTransfer_ApplyFee_ShouldFail(decimal startingBalance, decimal feeAmount)
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new TransferFundsCommand(1, 2, feeAmount);
            var sourceAccount = new Account(1, startingBalance, AccountType.Spend);
            var destAccount = new Account(2, 0, AccountType.Spend);
            var transferContext = new TransferContext(sourceAccount, destAccount, timeProvider.GetUtcNow());

            // Act
            var act = () => TransferFundsHandler.Handle(command, transferContext);

            // Assert
            using var _ = new AssertionScope();
            act.Should()
                .Throw<InsufficientFundsException>();
        }
    }

    public class CreditAccount
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
            var command = new TransferFundsCommand(1, 2, feeAmount);
            var sourceAccount = new Account(1, startingBalance, AccountType.Borrow);
            var destAccount = new Account(2, 0, AccountType.Borrow);
            var transferContext = new TransferContext(sourceAccount, destAccount, timeProvider.GetUtcNow());

            // Act
            var (customerWrite, revenueWrite, journalWrite, @event) =
                TransferFundsHandler.Handle(command, transferContext);

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
        public void GivenStartingBalanceLessThanFee_ApplyFee_ShouldSucceed(
            decimal startingBalance,
            decimal feeAmount,
            decimal expectedBalance
        )
        {
            // Arrange
            var timeProvider = new FakeTimeProvider();
            var command = new TransferFundsCommand(1, 2, feeAmount);
            var sourceAccount = new Account(1, startingBalance, AccountType.Borrow);
            var destAccount = new Account(2, 0, AccountType.Borrow);
            var transferContext = new TransferContext(sourceAccount, destAccount, timeProvider.GetUtcNow());

            // Act
            var act = () => TransferFundsHandler.Handle(command, transferContext);

            // Assert
            using var _ = new AssertionScope();
            act.Should()
                .NotThrow();
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
            var command = new TransferFundsCommand(1, 2, feeAmount);
            var sourceAccount = new Account(1, startingBalance, AccountType.Borrow);
            var destAccount = new Account(2, 0, AccountType.Borrow);
            var transferContext = new TransferContext(sourceAccount, destAccount, timeProvider.GetUtcNow());

            // Act
            var (customerWrite, revenueWrite, journalWrite, @event) =
                TransferFundsHandler.Handle(command, transferContext);

            // Assert
            using var _ = new AssertionScope();
            customerWrite.Entity.Balance.Should()
                .Be(expectedBalance);
        }
    }
}