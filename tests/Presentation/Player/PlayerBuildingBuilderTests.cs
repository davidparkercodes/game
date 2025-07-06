using FluentAssertions;
using Game.Presentation.Player;
using Moq;
using Xunit;
using PlayerClass = Game.Presentation.Player.Player;

namespace Game.Tests.Presentation.Player;

public class PlayerBuildingBuilderTests
{
    private Mock<PlayerClass> GetMockPlayer()
    {
        var mockPlayer = new Mock<PlayerClass>();
        return mockPlayer;
    }

    private PlayerBuildingBuilder GetBuildingBuilder(PlayerClass player)
    {
        return new PlayerBuildingBuilder(player);
    }

    [Fact]
    public void Constructor_WithValidPlayer_ShouldInitializeCorrectly()
    {
        var mockPlayer = GetMockPlayer();

        var builder = GetBuildingBuilder(mockPlayer.Object);

        builder.Should().NotBeNull();
        builder.IsInBuildMode.Should().BeFalse();
    }

    [Fact]
    public void IsInBuildMode_WhenNotInBuildMode_ShouldReturnFalse()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        builder.IsInBuildMode.Should().BeFalse();
    }

    [Fact]
    public void StartBuildMode_WithNullBuildingScene_ShouldNotThrow()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        var action = () => builder.StartBuildMode(null!);

        action.Should().NotThrow();
        builder.IsInBuildMode.Should().BeFalse();
    }

    [Fact]
    public void CancelBuildMode_WhenNotInBuildMode_ShouldNotThrow()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        var action = () => builder.CancelBuildMode();

        action.Should().NotThrow();
        builder.IsInBuildMode.Should().BeFalse();
    }

    [Fact]
    public void HandleInput_WithNullEvent_ShouldNotThrow()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        var action = () => builder.HandleInput(null!);

        action.Should().NotThrow();
    }

    [Fact]
    public void HandleInput_WithNonMouseEvent_ShouldNotThrow()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);
        var mockKeyEvent = new Mock<Godot.InputEventKey>();

        var action = () => builder.HandleInput(mockKeyEvent.Object);

        action.Should().NotThrow();
    }

    [Fact]
    public void PlayerBuildingBuilder_ShouldHaveCorrectPropertyTypes()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        // Test that IsInBuildMode property returns the correct type
        builder.IsInBuildMode.Should().Be(false);
    }

    [Fact]
    public void PlayerBuildingBuilder_ShouldHaveRequiredMethods()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        // Test that required methods exist
        var startBuildModeMethod = builder.GetType().GetMethod("StartBuildMode");
        var cancelBuildModeMethod = builder.GetType().GetMethod("CancelBuildMode");
        var handleInputMethod = builder.GetType().GetMethod("HandleInput");

        startBuildModeMethod.Should().NotBeNull();
        cancelBuildModeMethod.Should().NotBeNull();
        handleInputMethod.Should().NotBeNull();
    }

    [Fact]
    public void PlayerBuildingBuilder_BuildBuildingMethod_ShouldExist()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        // Test that the private BuildBuilding method exists
        var buildBuildingMethod = builder.GetType().GetMethod("BuildBuilding", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        buildBuildingMethod.Should().NotBeNull();
    }

    [Fact]
    public void PlayerBuildingBuilder_PlayConstructionSoundMethod_ShouldExist()
    {
        var mockPlayer = GetMockPlayer();
        var builder = GetBuildingBuilder(mockPlayer.Object);

        // Test that the private PlayConstructionSound method exists
        var playConstructionSoundMethod = builder.GetType().GetMethod("PlayConstructionSound", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        playConstructionSoundMethod.Should().NotBeNull();
    }
}
