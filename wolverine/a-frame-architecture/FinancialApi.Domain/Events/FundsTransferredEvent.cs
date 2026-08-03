namespace FinancialApi.Domain.Events;

public record FundsTransferredEvent(int SourceAccountId, int DestinationAccountId, decimal Amount);