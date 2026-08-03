using FinancialApi.Application.Handlers;
using FinancialApi.Application.Interfaces;
using FinancialApi.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;
using Wolverine;

namespace FinancialApi.Infrastructure.Tests;

public class WolverineTestFixture : IDisposable
{
    public IHost Host { get; }

    public WolverineTestFixture()
    {
        // We build and start the host ONCE here
        Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IAccountQuery>(_ => new FakeAccountQuery());
                    services.AddSingleton<TimeProvider>(_ => new FakeTimeProvider());
                    services.AddSingleton<IEventTracker>(_ => new DemoEventStore());
                }
            )
            .UseWolverine(opts => { opts.Discovery.IncludeAssembly(typeof(ApplyFeeHandler).Assembly); })
            .Start();
    }

    public void Dispose()
    {
        Host.Dispose();
    }
}