using System;
using Xunit;
using FluentAssertions;
using Moq;
using Game.Domain.Shared.Interfaces;
using Game.Domain.ValueObjects;

namespace Game.Tests.Domain.Shared;

public class DomainInterfacesTests
{
    [Fact]
    public void IStatsProvider_ShouldBeImplementable()
    {
        var mockStatsProvider = new Mock<IStatsProvider>();
        var buildingStats = BuildingStats.CreateDefault();
        
        mockStatsProvider
            .Setup(sp => sp.GetBuildingStats("BasicTurret"))
            .Returns(buildingStats);
        
        mockStatsProvider
            .Setup(sp => sp.HasBuildingStats("BasicTurret"))
            .Returns(true);

        var statsProvider = mockStatsProvider.Object;
        
        statsProvider.Should().NotBeNull();
        statsProvider.HasBuildingStats("BasicTurret").Should().BeTrue();
        var stats = statsProvider.GetBuildingStats("BasicTurret");
        stats.Should().Be(buildingStats);
    }

    [Fact]
    public void ISoundService_ShouldBeImplementable()
    {
        var mockSoundService = new Mock<ISoundService>();
        
        mockSoundService
            .Setup(ss => ss.IsSoundPlaying("test_sound"))
            .Returns(false);

        var soundService = mockSoundService.Object;
        
        soundService.Should().NotBeNull();
        soundService.IsSoundPlaying("test_sound").Should().BeFalse();
        
        Action playSound = () => soundService.PlaySound("test_sound");
        playSound.Should().NotThrow();
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
