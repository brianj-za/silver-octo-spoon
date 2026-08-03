using FinancialApi.Domain.Entities;

namespace FinancialApi.Application.Models;

public record FeeContext(Account Source, Account Destination, DateTimeOffset TimeStamp);