using System;
using FluentAssertions;
using Game.Domain.Buildings.Entities;
using Game.Domain.Buildings.Services;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Shared.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Buildings.Services;

public class BuildingZoneValidatorTests
{
    private static Building CreateTestBuilding(float x, float y) =>
        new BasicTower(
            new BuildingStats(100, 25, 150.0f, 60.0f, 800.0f, "shoot", "impact", "Test tower"),
            x, y);

    [Fact]
    public void CanPlaceBuilding_AtValidPosition_ShouldReturnTrue()
    {
        var validator = new BuildingZoneValidator();

        var canPlace = validator.CanPlaceBuilding(100.0f, 100.0f);

        canPlace.Should().BeTrue();
    }

    [Fact]
    public void CanPlaceBuilding_TooCloseToExisting_ShouldReturnFalse()
    {
        var validator = new BuildingZoneValidator(50.0f);
        var existingBuilding = CreateTestBuilding(100.0f, 100.0f);
        validator.AddBuilding(existingBuilding);

        var canPlace = validator.CanPlaceBuilding(120.0f, 120.0f); // Too close

        canPlace.Should().BeFalse();
    }

    [Fact]
    public void CanPlaceBuilding_InBlockedZone_ShouldReturnFalse()
    {
        var validator = new BuildingZoneValidator();
        validator.AddBlockedZone(100.0f, 100.0f);

        var canPlace = validator.CanPlaceBuilding(110.0f, 110.0f);

        canPlace.Should().BeFalse();
    }

    [Fact]
    public void GetNearbyBuildings_ShouldReturnBuildingsWithinRadius()
    {
        var validator = new BuildingZoneValidator();
        var nearBuilding = CreateTestBuilding(100.0f, 100.0f);
        var farBuilding = CreateTestBuilding(200.0f, 200.0f);
        validator.AddBuilding(nearBuilding);
        validator.AddBuilding(farBuilding);

        var nearbyBuildings = validator.GetNearbyBuildings(110.0f, 110.0f, 50.0f);

        nearbyBuildings.Should().ContainSingle().Which.Should().Be(nearBuilding);
    }
}
