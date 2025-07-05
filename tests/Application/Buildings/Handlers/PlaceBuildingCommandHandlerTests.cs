using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Moq;
using Godot;
using Game.Application.Buildings.Commands;
using Game.Application.Buildings.Handlers;
using Game.Infrastructure.Interfaces;

namespace Game.Tests.Application.Buildings.Handlers;

public class PlaceBuildingCommandHandlerTests
{
    private readonly Mock<IStatsService> _mockStatsService;
    private readonly Mock<IBuildingZoneService> _mockZoneService;
    private readonly PlaceBuildingCommandHandler _handler;

    public PlaceBuildingCommandHandlerTests()
    {
        _mockStatsService = new Mock<IStatsService>();
        _mockZoneService = new Mock<IBuildingZoneService>();
        _handler = new PlaceBuildingCommandHandler(_mockStatsService.Object, _mockZoneService.Object);
    }

    [Fact]
    public void Constructor_WithNullStatsService_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new PlaceBuildingCommandHandler(null, _mockZoneService.Object);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_WithNullZoneValidator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new PlaceBuildingCommandHandler(_mockStatsService.Object, null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task HandleAsync_WithNullCommand_ShouldReturnFailureResult()
    {
        // Act
        var result = await _handler.HandleAsync(null);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Command cannot be null");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyBuildingType_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new PlaceBuildingCommand("", new Vector2(100, 100));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Building type cannot be empty");
    }

    [Fact]
    public async Task HandleAsync_WithUnknownBuildingType_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new PlaceBuildingCommand("unknown_type", new Vector2(100, 100));
        _mockStatsService.Setup(x => x.GetBuildingStats("unknown_type")).Returns((BuildingStatsData)null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Unknown building type: unknown_type");
    }

    [Fact]
    public async Task HandleAsync_WithInvalidBuildingZone_ShouldReturnFailureResult()
    {
        // Arrange
        var command = new PlaceBuildingCommand("basic_turret", new Vector2(100, 100));
        var buildingStats = new BuildingStatsData { cost = 50 };
        
        _mockStatsService.Setup(x => x.GetBuildingStats("basic_turret")).Returns(buildingStats);
        _mockZoneService.Setup(x => x.CanBuildAt(new Vector2(100, 100))).Returns(false);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Cannot build at this location - invalid zone");
    }

    [Fact]
    public async Task HandleAsync_WithValidParameters_ShouldCallZoneValidator()
    {
        // Arrange
        var position = new Vector2(100, 100);
        var command = new PlaceBuildingCommand("basic_turret", position);
        var buildingStats = new BuildingStatsData { cost = 50 };
        
        _mockStatsService.Setup(x => x.GetBuildingStats("basic_turret")).Returns(buildingStats);
        _mockZoneService.Setup(x => x.CanBuildAt(position)).Returns(false);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockZoneService.Verify(x => x.CanBuildAt(position), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WithValidParameters_ShouldCallStatsService()
    {
        // Arrange
        var command = new PlaceBuildingCommand("basic_turret", new Vector2(100, 100));
        var buildingStats = new BuildingStatsData { cost = 50 };
        
        _mockStatsService.Setup(x => x.GetBuildingStats("basic_turret")).Returns(buildingStats);
        _mockZoneService.Setup(x => x.CanBuildAt(It.IsAny<Vector2>())).Returns(false);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _mockStatsService.Verify(x => x.GetBuildingStats("basic_turret"), Times.Once);
    }
}
