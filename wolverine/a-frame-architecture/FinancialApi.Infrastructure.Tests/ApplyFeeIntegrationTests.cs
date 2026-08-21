using FinancialApi.Application.Commands;
using FinancialApi.Application.Interfaces;
using FinancialApi.Domain.Entities;
using FinancialApi.Domain.Events;
using AwesomeAssertions;
using Wolverine;
using Wolverine.Tracking;

namespace FinancialApi.Infrastructure.Tests;

internal class FakeAccountQuery : IAccountQuery
{
    public Task<Account?> FindByIdAsync(int id) => Task.FromResult(new Account(id, 100.00m, AccountType.Spend))!;
}

public class ApplyFeeIntegrationTests(WolverineTestFixture fixture) : IClassFixture<WolverineTestFixture>
{
    [Fact]
    public async Task GivenValidCommand_WhenApplyingFee_ShouldPublishFeeAppliedEvent()
    {
        // Arrange
        var command = new ApplyFeeCommand(1, 100);

        // Act
        var session = await fixture.Host.InvokeMessageAndWaitAsync(command);

        // Assert
        session.Sent.AllMessages()
            .ShouldHaveMessageOfType<FeeAppliedEvent>();
        session.Sent.MessagesOf<FeeAppliedEvent>()
            .Count()
            .Should()
            .Be(1);
    }
}