using System.Threading.Tasks;
using FluentAssertions;
using Game.Application.Game.Commands;
using Game.Application.Game.Handlers;
using Xunit;

namespace Game.Application.Game.Handlers.Tests;

public class SpendMoneyCommandHandlerTests
{
    private readonly SpendMoneyCommandHandler _handler;

    public SpendMoneyCommandHandlerTests()
    {
        _handler = new SpendMoneyCommandHandler();
    }

    [Fact]
    public async Task HandleAsync_WithValidAmount_ShouldProcessSuccessfully()
    {
        var command = new SpendMoneyCommand(50, "Building tower");

        var result = await _handler.HandleAsync(command);

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.RemainingMoney.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task HandleAsync_WithNegativeAmount_ShouldFail()
    {
        var command = new SpendMoneyCommand(-10, "Invalid transaction");

        var result = await _handler.HandleAsync(command);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Amount cannot be negative");
    }

    [Fact]
    public async Task HandleAsync_WithNullCommand_ShouldFail()
    {
        var result = await _handler.HandleAsync(null!);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Command cannot be null");
        result.RemainingMoney.Should().Be(0);
    }
}
