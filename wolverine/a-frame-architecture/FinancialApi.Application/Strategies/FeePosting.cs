using FinancialApi.Application.Commands;
using FinancialApi.Application.Models;
using FinancialApi.Domain.Services;

namespace FinancialApi.Application.Strategies;

public sealed class FeePosting
{
    public PostingResult Apply(ApplyFeeCommand cmd, FeeContext ctx) => Ledger.PostBalanced(
        cmd.Amount,
        $"Fee {ctx.Destination.Id} → revenue {ctx.Destination.Id}",
        ctx.TimeStamp,
        ctx.Source,
        ctx.Destination
    );
}