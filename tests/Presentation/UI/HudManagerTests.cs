using FluentAssertions;
using Game.Presentation.UI;
using Moq;
using Xunit;

namespace Game.Tests.Presentation.UI;

public class HudManagerTests
{
    private HudManager GetHudManager()
    {
        return new HudManager();
    }

    private Mock<Hud> GetMockHud()
    {
        var mockHud = new Mock<Hud>();
        // Setup basic mock properties that HudManager uses
        mockHud.Setup(h => h.UpdateMoney(It.IsAny<int>()));
        mockHud.Setup(h => h.UpdateLives(It.IsAny<int>()));
        mockHud.Setup(h => h.UpdateWave(It.IsAny<int>()));
        mockHud.Setup(h => h.ShowBuildingStats(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<float>(), It.IsAny<float>()));
        mockHud.Setup(h => h.HideBuildingStats());
        mockHud.Setup(h => h.ShowSkipButton());
        mockHud.Setup(h => h.HideSkipButton());
        return mockHud;
    }

    [Fact]
    public void Initialize_WithValidHud_ShouldSetHudReference()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();

        hudManager.Initialize(mockHud.Object);

        hudManager.IsInitialized().Should().BeTrue();
        hudManager.GetHud().Should().Be(mockHud.Object);
    }

    [Fact]
    public void UpdateMoney_WhenInitialized_ShouldCallHudUpdateMoney()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.UpdateMoney(150);

        mockHud.Verify(h => h.UpdateMoney(150), Times.Once);
    }

    [Fact]
    public void UpdateMoney_WhenNotInitialized_ShouldNotThrow()
    {
        var hudManager = GetHudManager();

        var action = () => hudManager.UpdateMoney(150);

        action.Should().NotThrow();
    }

    [Fact]
    public void UpdateLives_WhenInitialized_ShouldCallHudUpdateLives()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.UpdateLives(10);

        mockHud.Verify(h => h.UpdateLives(10), Times.Once);
    }

    [Fact]
    public void UpdateWave_WhenInitialized_ShouldCallHudUpdateWave()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.UpdateWave(3);

        mockHud.Verify(h => h.UpdateWave(3), Times.Once);
    }

    [Fact]
    public void ShowBuildingStats_WhenInitialized_ShouldCallHudShowBuildingStats()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.ShowBuildingStats("Test Tower", 100, 50, 150.0f, 1.5f);

        mockHud.Verify(h => h.ShowBuildingStats("Test Tower", 100, 50, 150.0f, 1.5f), Times.Once);
    }

    [Fact]
    public void HideBuildingStats_WhenInitialized_ShouldCallHudHideBuildingStats()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.HideBuildingStats();

        mockHud.Verify(h => h.HideBuildingStats(), Times.Once);
    }

    [Fact]
    public void ShowSkipButton_WhenInitialized_ShouldCallHudShowSkipButton()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        hudManager.Initialize(mockHud.Object);

        hudManager.ShowSkipButton();

        mockHud.Verify(h => h.ShowSkipButton(), Times.Once);
    }

    [Fact]
    public void IsInitialized_WhenNotInitialized_ShouldReturnFalse()
    {
        var hudManager = GetHudManager();

        hudManager.IsInitialized().Should().BeFalse();
    }

    [Fact]
    public void SetWaveButtonState_WhenInitializedWithSkipButton_ShouldUpdateButtonState()
    {
        var hudManager = GetHudManager();
        var mockHud = GetMockHud();
        var mockButton = new Mock<Godot.Button>();
        mockHud.Setup(h => h.SkipButton).Returns(mockButton.Object);
        hudManager.Initialize(mockHud.Object);

        hudManager.SetWaveButtonState("Start Wave 2", true);

        mockButton.VerifySet(b => b.Text = "Start Wave 2", Times.Once);
        mockButton.VerifySet(b => b.Disabled = false, Times.Once);
    }
}
