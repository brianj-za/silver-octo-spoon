namespace FinancialApi.Domain.Exceptions;

public class InvalidStartingBalanceException(string message) : GaapViolationException(message);