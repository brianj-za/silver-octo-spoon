using FinancialApi.Domain.Exceptions;

namespace FinancialApi.Domain.Entities;

public enum AccountType
{
    Spend,
    Borrow,
    Revenue
}

public record Account
{
    public Account(int Id, decimal Balance, AccountType Type)
    {
        this.Id = Id;
        this.Balance = Balance;
        this.Type = Type;

        if (Balance < 0 && Type is AccountType.Spend or AccountType.Revenue)
        {
            throw new InvalidStartingBalanceException("Insufficient funds.");
        }
    }

    public Account ApplyPosting(decimal amount, EntryType entryType)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive.");
        }

        var newBalance = entryType == EntryType.Credit ? Balance + amount : Balance - amount;

        if (newBalance < 0 && Type is AccountType.Spend or AccountType.Revenue)
        {
            throw new InsufficientFundsException("Insufficient funds.");
        }

        return this with { Balance = newBalance };
    }

    public int Id { get; init; }
    public decimal Balance { get; init; }
    public AccountType Type { get; init; }

    public void Deconstruct(out int Id, out decimal Balance, out AccountType Type)
    {
        Id = this.Id;
        Balance = this.Balance;
        Type = this.Type;
    }
}