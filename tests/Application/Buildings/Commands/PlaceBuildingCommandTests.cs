using System;
using Xunit;
using FluentAssertions;
using Godot;
using Game.Application.Buildings.Commands;

namespace Game.Tests.Application.Buildings.Commands;

public class PlaceBuildingCommandTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateCommand()
    {
        // Arrange
        var buildingType = "basic_turret";
        var position = new Vector2(100, 200);
        var playerId = 1;

        // Act
        var command = new PlaceBuildingCommand(buildingType, position, playerId);

        // Assert
        command.BuildingType.Should().Be(buildingType);
        command.Position.Should().Be(position);
        command.PlayerId.Should().Be(playerId);
    }

    [Fact]
    public void Constructor_WithDefaultPlayerId_ShouldUseZero()
    {
        // Arrange
        var buildingType = "sniper_turret";
        var position = new Vector2(50, 100);

        // Act
        var command = new PlaceBuildingCommand(buildingType, position);

        // Assert
        command.BuildingType.Should().Be(buildingType);
        command.Position.Should().Be(position);
        command.PlayerId.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithNullBuildingType_ShouldThrowArgumentNullException()
    {
        // Arrange
        var position = new Vector2(100, 100);

        // Act & Assert
        var action = () => new PlaceBuildingCommand(null, position);
        action.Should().Throw<ArgumentNullException>();
    }
}

public class PlaceBuildingResultTests
{
    [Fact]
    public void Successful_WithValidParameters_ShouldCreateSuccessResult()
    {
        // Arrange
        var buildingId = 123;
        var costPaid = 50;

        // Act
        var result = PlaceBuildingResult.Successful(buildingId, costPaid);

        // Assert
        result.Success.Should().BeTrue();
        result.ErrorMessage.Should().BeNull();
        result.BuildingId.Should().Be(buildingId);
        result.CostPaid.Should().Be(costPaid);
    }

    [Fact]
    public void Failed_WithErrorMessage_ShouldCreateFailureResult()
    {
        // Arrange
        var errorMessage = "Not enough money";

        // Act
        var result = PlaceBuildingResult.Failed(errorMessage);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be(errorMessage);
        result.BuildingId.Should().Be(0);
        result.CostPaid.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithAllParameters_ShouldSetAllProperties()
    {
        // Arrange
        var success = true;
        var errorMessage = "Test error";
        var buildingId = 456;
        var costPaid = 75;

        // Act
        var result = new PlaceBuildingResult(success, errorMessage, buildingId, costPaid);

        // Assert
        result.Success.Should().Be(success);
        result.ErrorMessage.Should().Be(errorMessage);
        result.BuildingId.Should().Be(buildingId);
        result.CostPaid.Should().Be(costPaid);
    }
}
