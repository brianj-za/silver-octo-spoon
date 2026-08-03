namespace FinancialApi.Application.Commands;

public record ApplyFeeCommand(int AccountId, decimal Amount);