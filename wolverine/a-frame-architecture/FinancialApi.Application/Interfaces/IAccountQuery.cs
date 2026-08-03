using FinancialApi.Domain.Entities;

namespace FinancialApi.Application.Interfaces;

public interface IAccountQuery
{
    Task<Account?> FindByIdAsync(int id);
}