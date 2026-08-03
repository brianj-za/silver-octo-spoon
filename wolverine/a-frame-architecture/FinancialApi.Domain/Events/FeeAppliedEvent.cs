namespace FinancialApi.Domain.Events;

public record FeeAppliedEvent(int AccountId, decimal Amount);