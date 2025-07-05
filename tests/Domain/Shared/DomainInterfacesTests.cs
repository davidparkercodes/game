using System;
using Xunit;
using FluentAssertions;
using Moq;
using Game.Domain.Buildings.Services;
using Game.Domain.Enemies.Services;
using Game.Domain.Buildings.ValueObjects;

namespace Game.Tests.Domain.Shared;

public class DomainInterfacesTests
{
    [Fact]
    public void IBuildingStatsProvider_ShouldBeImplementable()
    {
        var mockBuildingStatsProvider = new Mock<IBuildingStatsProvider>();
        var buildingStats = BuildingStats.CreateDefault();
        
        mockBuildingStatsProvider
            .Setup(sp => sp.GetBuildingStats("BasicTurret"))
            .Returns(buildingStats);
        
        mockBuildingStatsProvider
            .Setup(sp => sp.HasBuildingStats("BasicTurret"))
            .Returns(true);

        var statsProvider = mockBuildingStatsProvider.Object;
        
        statsProvider.Should().NotBeNull();
        statsProvider.HasBuildingStats("BasicTurret").Should().BeTrue();
        var stats = statsProvider.GetBuildingStats("BasicTurret");
        stats.Should().Be(buildingStats);
    }

    [Fact]
    public void IEnemyStatsProvider_ShouldBeImplementable()
    {
        var mockEnemyStatsProvider = new Mock<IEnemyStatsProvider>();
        
        mockEnemyStatsProvider
            .Setup(sp => sp.HasEnemyStats("BasicEnemy"))
            .Returns(true);

        var statsProvider = mockEnemyStatsProvider.Object;
        
        statsProvider.Should().NotBeNull();
        statsProvider.HasEnemyStats("BasicEnemy").Should().BeTrue();
    }


    [Fact]
    public void IBuildingService_ShouldBeImplementable()
    {
        var mockBuildingService = new Mock<IBuildingService>();
        
        mockBuildingService
            .Setup(bs => bs.CanPlaceBuilding("BasicTurret", 100f, 100f))
            .Returns(true);
        
        mockBuildingService
            .Setup(bs => bs.GetBuildingCount("BasicTurret"))
            .Returns(5);

        var buildingService = mockBuildingService.Object;
        
        buildingService.Should().NotBeNull();
        buildingService.CanPlaceBuilding("BasicTurret", 100f, 100f).Should().BeTrue();
        buildingService.GetBuildingCount("BasicTurret").Should().Be(5);
    }

    [Fact]
    public void IWaveService_ShouldBeImplementable()
    {
        var mockWaveService = new Mock<IWaveService>();
        
        mockWaveService
            .Setup(ws => ws.IsWaveActive())
            .Returns(true);
        
        mockWaveService
            .Setup(ws => ws.GetCurrentWaveNumber())
            .Returns(3);

        var waveService = mockWaveService.Object;
        
        waveService.Should().NotBeNull();
        waveService.IsWaveActive().Should().BeTrue();
        waveService.GetCurrentWaveNumber().Should().Be(3);
    }
}
