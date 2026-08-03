using FinancialApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialApi.Infrastructure;

public class AccountDbContext(DbContextOptions<AccountDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<BalancedJournalEntry> JournalEntries => Set<BalancedJournalEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Account>()
            .HasData(
                new Account(1, 10000000.00m, AccountType.Borrow),
                new Account(2, 50000000.00m, AccountType.Spend),
                new Account(99999, 0.00m, AccountType.Revenue)
            );

        modelBuilder.Entity<BalancedJournalEntry>(builder =>
            {
                builder.HasKey(x => x.Id);
                builder.OwnsMany(
                    x => x.Lines,
                    line =>
                    {
                        line.WithOwner()
                            .HasForeignKey("JournalEntryId");
                        line.Property<int>("Id")
                            .ValueGeneratedOnAdd();
                        line.HasKey("Id");
                        line.Property(x => x.Type)
                            .HasConversion<string>();
                    }
                );
            }
        );
    }
}