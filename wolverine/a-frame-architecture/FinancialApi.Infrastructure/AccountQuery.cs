using FinancialApi.Application.Interfaces;
using FinancialApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialApi.Infrastructure;

public class AccountQuery(AccountDbContext db) : IAccountQuery
{
    public async Task<Account?> FindByIdAsync(int id)
    {
        return await db.Accounts.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}