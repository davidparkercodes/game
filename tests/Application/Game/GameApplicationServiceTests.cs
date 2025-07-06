using System;
using System.Threading.Tasks;
using FluentAssertions;
using Game.Application.Game;
using Game.Application.Game.Commands;
using Game.Application.Rounds.Commands;
using Game.Application.Shared.Cqrs;
using Moq;
using Xunit;

namespace Game.Application.Game.Tests;

public class GameApplicationServiceTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly GameApplicationService _service;

    public GameApplicationServiceTests()
    {
        _mockMediator = new Mock<IMediator>();
        _service = new GameApplicationService(_mockMediator.Object);
    }

    [Fact]
    public async Task SpendMoneyAsync_ShouldDelegateToMediator()
    {
        var expectedResult = SpendMoneyResult.Successful(450);
        _mockMediator.Setup(x => x.SendAsync<SpendMoneyResult>(It.IsAny<ICommand<SpendMoneyResult>>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(expectedResult);

        var result = await _service.SpendMoneyAsync(50, "Test purchase");

        result.Should().Be(expectedResult);
        _mockMediator.Verify(x => x.SendAsync<SpendMoneyResult>(It.IsAny<ICommand<SpendMoneyResult>>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StartRoundAsync_ShouldDelegateToMediator()
    {
        var expectedResult = StartRoundResult.Successful(2, "Active");
        _mockMediator.Setup(x => x.SendAsync<StartRoundResult>(It.IsAny<ICommand<StartRoundResult>>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(expectedResult);

        var result = await _service.StartRoundAsync(2);

        result.Should().Be(expectedResult);
        _mockMediator.Verify(x => x.SendAsync<StartRoundResult>(It.IsAny<ICommand<StartRoundResult>>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task TrySpendMoney_WhenSuccessful_ShouldReturnResult()
    {
        var expectedResult = SpendMoneyResult.Successful(400);
        _mockMediator.Setup(x => x.SendAsync<SpendMoneyResult>(It.IsAny<ICommand<SpendMoneyResult>>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ReturnsAsync(expectedResult);

        var result = await _service.TrySpendMoney(100, "Safe purchase");

        result.Success.Should().BeTrue();
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task TrySpendMoney_WhenException_ShouldReturnFailureResult()
    {
        _mockMediator.Setup(x => x.SendAsync<SpendMoneyResult>(It.IsAny<ICommand<SpendMoneyResult>>(), It.IsAny<System.Threading.CancellationToken>()))
                    .ThrowsAsync(new InvalidOperationException("Test exception"));

        var result = await _service.TrySpendMoney(100, "Failing purchase");

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Error spending money");
        result.ErrorMessage.Should().Contain("Test exception");
    }
}
