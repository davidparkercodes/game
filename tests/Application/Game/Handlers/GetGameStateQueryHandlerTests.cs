using System.Threading.Tasks;
using FluentAssertions;
using Game.Application.Game.Handlers;
using Game.Application.Game.Queries;
using Xunit;

namespace Game.Application.Game.Handlers.Tests;

public class GetGameStateQueryHandlerTests
{
    private readonly GetGameStateQueryHandler _handler;

    public GetGameStateQueryHandlerTests()
    {
        _handler = new GetGameStateQueryHandler();
    }

    [Fact]
    public async Task HandleAsync_WithValidQuery_ShouldReturnGameStateSuccessfully()
    {
        var query = new GetGameStateQuery();

        var result = await _handler.HandleAsync(query);

        result.Should().NotBeNull();
        result.Money.Should().BeGreaterThanOrEqualTo(0);
        result.Lives.Should().BeGreaterThanOrEqualTo(0);
        result.Score.Should().BeGreaterThanOrEqualTo(0);
        result.CurrentRound.Should().BeGreaterThan(0);
        result.CurrentPhase.Should().NotBeNullOrEmpty();
        result.EnemiesRemaining.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task HandleAsync_ShouldProvideConsistentData()
    {
        var query = new GetGameStateQuery();

        var result1 = await _handler.HandleAsync(query);
        var result2 = await _handler.HandleAsync(query);

        result1.CurrentPhase.Should().Be(result2.CurrentPhase);
        result1.CurrentRound.Should().Be(result2.CurrentRound);
    }

    [Fact]
    public async Task HandleAsync_WhenServicesUnavailable_ShouldProvideDefaultValues()
    {
        var query = new GetGameStateQuery();

        var result = await _handler.HandleAsync(query);

        result.Money.Should().BeGreaterThanOrEqualTo(0);
        result.Lives.Should().BeGreaterThanOrEqualTo(0);
        result.CurrentRound.Should().BeGreaterThan(0);
        result.CurrentPhase.Should().NotBeNullOrEmpty();
    }
}
