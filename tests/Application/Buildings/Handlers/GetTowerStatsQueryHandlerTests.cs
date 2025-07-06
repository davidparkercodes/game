using System.Threading.Tasks;
using FluentAssertions;
using Game.Application.Buildings.Handlers;
using Game.Application.Buildings.Queries;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Moq;
using Xunit;

namespace Game.Application.Buildings.Handlers.Tests;

public class GetTowerStatsQueryHandlerTests
{
    private readonly Mock<IBuildingStatsProvider> _mockStatsProvider;
    private readonly Mock<IBuildingTypeRegistry> _mockTypeRegistry;
    private readonly GetTowerStatsQueryHandler _handler;

    public GetTowerStatsQueryHandlerTests()
    {
        _mockStatsProvider = new Mock<IBuildingStatsProvider>();
        _mockTypeRegistry = new Mock<IBuildingTypeRegistry>();
        _handler = new GetTowerStatsQueryHandler(_mockStatsProvider.Object, _mockTypeRegistry.Object);
    }

    [Fact]
    public async Task HandleAsync_WithValidTowerType_ShouldReturnStatsSuccessfully()
    {
        var query = new GetTowerStatsQuery("basic_tower");
        var buildingStats = new BuildingStats(100, 50, 150.0f, 30.0f, 300.0f, "shoot.wav", "impact.wav", "Basic tower");
        
        _mockTypeRegistry.Setup(x => x.IsValidConfigKey("basic_tower")).Returns(true);
        _mockStatsProvider.Setup(x => x.GetBuildingStats("basic_tower")).Returns(buildingStats);

        var result = await _handler.HandleAsync(query);

        result.IsAvailable.Should().BeTrue();
        result.TowerType.Should().Be("basic_tower");
        result.Cost.Should().Be(100);
        result.Damage.Should().Be(50);
        result.Range.Should().Be(150.0f);
        result.AttackSpeed.Should().Be(30.0f);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidTowerType_ShouldReturnNotFound()
    {
        var query = new GetTowerStatsQuery("invalid_tower");
        
        _mockTypeRegistry.Setup(x => x.IsValidConfigKey("invalid_tower")).Returns(false);

        var result = await _handler.HandleAsync(query);

        result.IsAvailable.Should().BeFalse();
        result.TowerType.Should().Be("invalid_tower");
        result.Description.Should().Be("Not found");
    }

    [Fact]
    public async Task HandleAsync_WithEmptyTowerType_ShouldReturnNotFound()
    {
        var query = new GetTowerStatsQuery("");

        var result = await _handler.HandleAsync(query);

        result.IsAvailable.Should().BeFalse();
        result.TowerType.Should().Be("");
        result.Description.Should().Be("Not found");
    }
}
