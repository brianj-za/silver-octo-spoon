using FinancialApi.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FinancialApi;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (exception is not (InvalidOperationException or ArgumentException or GaapViolationException))
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Domain Validation Error",
            Detail = exception.Message,
            Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1"
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}