using System;
using FluentAssertions;
using Game.Domain.Buildings.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Buildings.ValueObjects;

public class BuildingTypeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBuildingType()
    {
        var buildingType = new BuildingType("tower_basic", "BasicTower", "Basic Tower", "Tower");

        buildingType.InternalId.Should().Be("tower_basic");
        buildingType.ConfigKey.Should().Be("BasicTower");
        buildingType.DisplayName.Should().Be("Basic Tower");
        buildingType.Category.Should().Be("Tower");
    }

    [Fact]
    public void Constructor_WithNullInternalId_ShouldThrowArgumentException()
    {
        var action = () => new BuildingType(null!, "config", "display", "category");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Internal ID cannot be null or empty*");
    }

    [Fact]
    public void Equals_ShouldBeBasedOnInternalId()
    {
        var type1 = new BuildingType("tower_basic", "Config1", "Display1", "Category1");
        var type2 = new BuildingType("tower_basic", "Config2", "Display2", "Category2");
        var type3 = new BuildingType("tower_sniper", "Config1", "Display1", "Category1");

        type1.Should().Be(type2);
        type1.Should().NotBe(type3);
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var buildingType = new BuildingType("tower_basic", "BasicTowerConfig", "Basic Tower", "Defense");

        var result = buildingType.ToString();

        result.Should().Be("Basic Tower (tower_basic)");
    }
}
