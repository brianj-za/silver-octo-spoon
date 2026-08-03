namespace FinancialApi.Domain.Entities;

public record BalancedJournalEntry
{
    public Guid Id { get; init; }
    public string Description { get; init; } = null!;
    public DateTimeOffset CreatedAt { get; init; }
    public List<JournalLine> Lines { get; init; } = [];

    internal BalancedJournalEntry() { }
    
    internal BalancedJournalEntry(Guid id, string description, DateTimeOffset createdAt, List<JournalLine> lines)
    {
        Lines = lines;
        Id = id;
        Description = description;
        CreatedAt = createdAt;
    }
}