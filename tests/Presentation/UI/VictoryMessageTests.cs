using FluentAssertions;
using Game.Presentation.UI;
using Game.Infrastructure.Game.Services;
using Moq;
using Xunit;

namespace Game.Tests.Presentation.UI;

public class VictoryMessageTests
{
    private Mock<Hud> GetMockHud()
    {
        var mockHud = new Mock<Hud>();
        mockHud.Setup(h => h.ShowVictoryMessage());
        mockHud.Setup(h => h.HideVictoryMessage());
        return mockHud;
    }

    [Fact]
    public void GameService_MarkGameAsWon_ShouldSetVictoryFlag()
    {
        var gameService = GameService.Instance;
        gameService.Reset();

        gameService.MarkGameAsWon();

        gameService.IsGameWon().Should().BeTrue();
        gameService.IsGameActive.Should().BeFalse();
    }

    [Fact]
    public void GameService_Reset_ShouldClearVictoryFlag()
    {
        var gameService = GameService.Instance;
        gameService.MarkGameAsWon();
        gameService.IsGameWon().Should().BeTrue();

        gameService.Reset();

        gameService.IsGameWon().Should().BeFalse();
    }

    [Fact]
    public void HudManager_ShowVictoryMessage_WhenInitialized_ShouldCallHudShowVictoryMessage()
    {
        var hudManager = new HudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.ShowVictoryMessage();

        mockHud.Verify(h => h.ShowVictoryMessage(), Times.Once);
    }

    [Fact]
    public void HudManager_HideVictoryMessage_WhenInitialized_ShouldCallHudHideVictoryMessage()
    {
        var hudManager = new HudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.HideVictoryMessage();

        mockHud.Verify(h => h.HideVictoryMessage(), Times.Once);
    }

    [Fact]
    public void HudManager_ShowVictoryMessage_WhenNotInitialized_ShouldNotThrow()
    {
        var hudManager = new HudManager();

        var action = () => hudManager.ShowVictoryMessage();

        action.Should().NotThrow();
    }

    [Fact]
    public void HudManager_HideVictoryMessage_WhenNotInitialized_ShouldNotThrow()
    {
        var hudManager = new HudManager();

        var action = () => hudManager.HideVictoryMessage();

        action.Should().NotThrow();
    }
}
