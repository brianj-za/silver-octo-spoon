namespace FinancialApi.Domain.Exceptions;

public class GaapViolationException(string message) : Exception(message);