namespace FinancialApi.Application.Commands;

public record TransferFundsCommand(int SourceAccountId, int DestinationAccountId, decimal Amount);