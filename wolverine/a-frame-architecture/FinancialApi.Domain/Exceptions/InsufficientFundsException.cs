namespace FinancialApi.Domain.Exceptions;

public class InsufficientFundsException(string message) : GaapViolationException(message);