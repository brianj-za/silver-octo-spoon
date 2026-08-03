using FinancialApi.Domain.Entities;

namespace FinancialApi.Application.Models;

public record TransferContext(Account Source, Account Destination, DateTimeOffset TimeStamp);