using System.Threading.Tasks;
using FluentAssertions;
using Game.Application.Buildings.Commands;
using Game.Application.Buildings.Handlers;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Shared.ValueObjects;
using Moq;
using Xunit;

namespace Game.Application.Buildings.Handlers.Tests;

public class PlaceBuildingCommandHandlerTests
{
    private readonly Mock<IBuildingStatsProvider> _mockStatsProvider;
    private readonly Mock<IBuildingZoneService> _mockZoneService;
    private readonly Mock<IBuildingTypeRegistry> _mockTypeRegistry;
    private readonly PlaceBuildingCommandHandler _handler;

    public PlaceBuildingCommandHandlerTests()
    {
        _mockStatsProvider = new Mock<IBuildingStatsProvider>();
        _mockZoneService = new Mock<IBuildingZoneService>();
        _mockTypeRegistry = new Mock<IBuildingTypeRegistry>();
        _handler = new PlaceBuildingCommandHandler(_mockStatsProvider.Object, _mockZoneService.Object, _mockTypeRegistry.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidInput_ShouldSucceed()
    {
        var command = new PlaceBuildingCommand("basic_tower", new Position(100, 100));
        var buildingStats = new BuildingStats(100, 50, 2.0f, 1.5f, 300.0f, "shoot.wav", "impact.wav", "Basic tower");
        
        _mockTypeRegistry.Setup(x => x.IsValidConfigKey("basic_tower")).Returns(true);
        _mockStatsProvider.Setup(x => x.GetBuildingStats("basic_tower")).Returns(buildingStats);
        _mockZoneService.Setup(x => x.CanBuildAt(It.IsAny<Position>())).Returns(true);

        var result = await _handler.HandleAsync(command);

        result.Success.Should().BeTrue();
        result.BuildingId.Should().BeGreaterThan(0);
        result.CostPaid.Should().Be(100);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidBuildingType_ShouldFail()
    {
        var command = new PlaceBuildingCommand("invalid_tower", new Position(100, 100));
        
        _mockTypeRegistry.Setup(x => x.IsValidConfigKey("invalid_tower")).Returns(false);

        var result = await _handler.HandleAsync(command);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Unknown building type");
    }

    [Fact]
    public async Task HandleAsync_WithInvalidZone_ShouldFail()
    {
        var command = new PlaceBuildingCommand("basic_tower", new Position(100, 100));
        
        _mockTypeRegistry.Setup(x => x.IsValidConfigKey("basic_tower")).Returns(true);
        _mockZoneService.Setup(x => x.CanBuildAt(It.IsAny<Position>())).Returns(false);

        var result = await _handler.HandleAsync(command);

        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Cannot build at this location");
    }
}
