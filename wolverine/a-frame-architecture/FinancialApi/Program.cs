using FinancialApi;
using FinancialApi.Application.Commands;
using FinancialApi.Application.Handlers;
using FinancialApi.Application.Interfaces;
using FinancialApi.Domain.Events;
using FinancialApi.Infrastructure;
using FinancialApi.ServiceDefaults;
using JasperFx.Resources;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Sqlite;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();

var connectionString = "Data Source=demo.db";

builder.Services.AddDbContext<AccountDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddScoped<IAccountQuery, AccountQuery>();
builder.Services.AddScoped<IReversalQuery, ReversalQuery>();
builder.Services.AddSingleton<IEventTracker, DemoEventStore>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddServiceDefaults();

builder.Host.UseWolverine(opts =>
    {
        opts.Discovery.IncludeAssembly(typeof(DemoEventStore).Assembly);
        opts.Discovery.IncludeAssembly(typeof(ApplyFeeHandler).Assembly);
        opts.PersistMessagesWithSqlite(connectionString);
        opts.UseEntityFrameworkCoreTransactions();
        opts.Policies.AutoApplyTransactions();
    }
);
builder.Host.UseResourceSetupOnStartup();

var app = builder.Build();

app.UseExceptionHandler();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AccountDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost(
    "/accounts/apply-fee",
    async (ApplyFeeCommand cmd, IMessageBus bus) =>
    {
        await bus.InvokeAsync(cmd);
        return Results.Accepted();
    }
);

app.MapPost(
    "/accounts/transfer",
    async (TransferFundsCommand cmd, IMessageBus bus) =>
    {
        await bus.InvokeAsync(cmd);
        return Results.Accepted();
    }
);

app.MapPost(
    "/journal/reverse",
    async (ReverseJournalCommand cmd, IMessageBus bus) =>
    {
        await bus.InvokeAsync(cmd);
        return Results.Accepted();
    }
);

app.MapGet(
    "/events",
    (IEventTracker tracker) =>
    {
        if (tracker is DemoEventStore store)
        {
            return Results.Ok(store.CapturedEvents);
        }

        return Results.NotFound();
    }
);

app.Run();