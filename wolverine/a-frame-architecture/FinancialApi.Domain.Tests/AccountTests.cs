using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Exceptions;
using FluentAssertions;

namespace FinancialApi.Domain.Tests;

public class AccountTests
{
    [Theory]
    [InlineData(AccountType.Borrow)]
    public void CreateBorrowAccountType_ShouldAllowNegativeBalance(AccountType accountType)
    {
        var act = () => new Account(1, -100, accountType);

        act()
            .Balance.Should()
            .BeNegative();
    }

    [Theory]
    [InlineData(AccountType.Revenue)]
    [InlineData(AccountType.Spend)]
    public void CreateDebitAccountType_ShouldNotAllowNegativeBalance(AccountType accountType)
    {
        var act = () => new Account(1, -100, accountType);

        act.Should()
            .Throw<InvalidStartingBalanceException>();
    }

    [Theory]
    [InlineData(AccountType.Borrow)]
    public void GivenCreditAccountType_ShouldAllowNegativeBalance(AccountType accountType)
    {
        var account = new Account(1, 0, accountType);

        var updatedAccount = account.ApplyPosting(100, EntryType.Debit);

        updatedAccount.Balance.Should()
            .BeNegative();
    }

    [Theory]
    [InlineData(AccountType.Revenue)]
    [InlineData(AccountType.Spend)]
    public void GivenDebitAccountType_ShouldNotAllowNegativeBalance(AccountType accountType)
    {
        var account = new Account(1, 0, accountType);

        var act = () => account.ApplyPosting(100, EntryType.Debit);

        act.Should()
            .Throw<InsufficientFundsException>();
    }
}