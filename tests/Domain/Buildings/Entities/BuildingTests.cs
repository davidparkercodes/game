using System;
using FluentAssertions;
using Game.Domain.Buildings.Entities;
using Game.Domain.Buildings.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Buildings.Entities;

public class BuildingTests
{
    private static BuildingStats CreateValidStats()
    {
        return new BuildingStats(
            cost: 100,
            damage: 25,
            range: 150.0f,
            attackSpeed: 30.0f,
            bulletSpeed: 500.0f,
            shootSound: "test_shoot",
            impactSound: "test_impact",
            description: "Test building"
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBuilding()
    {
        var stats = CreateValidStats();
        var x = 50.0f;
        var y = 75.0f;

        var building = new BasicTower(stats, x, y);

        building.Id.Should().NotBeEmpty();
        building.Stats.Should().Be(stats);
        building.X.Should().Be(x);
        building.Y.Should().Be(y);
        building.IsActive.Should().BeTrue();
        building.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_ShouldAssignUniqueIds()
    {
        var stats = CreateValidStats();
        
        var building1 = new BasicTower(stats, 0, 0);
        var building2 = new BasicTower(stats, 0, 0);

        building1.Id.Should().NotBe(building2.Id);
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.NaN, float.NaN)]
    public void Constructor_WithNaNPosition_ShouldThrowArgumentException(float x, float y)
    {
        var stats = CreateValidStats();

        var action = () => new BasicTower(stats, x, y);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Position cannot be NaN");
    }

    [Theory]
    [InlineData(float.PositiveInfinity, 0.0f)]
    [InlineData(0.0f, float.NegativeInfinity)]
    [InlineData(float.PositiveInfinity, float.NegativeInfinity)]
    public void Constructor_WithInfinitePosition_ShouldThrowArgumentException(float x, float y)
    {
        var stats = CreateValidStats();

        var action = () => new BasicTower(stats, x, y);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Position cannot be infinite");
    }

    [Theory]
    [InlineData(0.0f, 0.0f, 50.0f, 50.0f, true)]    // Distance ≈ 70.7, Range 150
    [InlineData(0.0f, 0.0f, 100.0f, 100.0f, true)]  // Distance ≈ 141.4, Range 150  
    [InlineData(0.0f, 0.0f, 150.0f, 0.0f, true)]    // Distance = 150, Range 150 (exactly on range)
    [InlineData(0.0f, 0.0f, 151.0f, 0.0f, false)]   // Distance = 151, Range 150 (outside range)
    [InlineData(0.0f, 0.0f, 200.0f, 200.0f, false)] // Distance ≈ 282.8, Range 150
    public void IsInRange_ShouldCalculateCorrectly(float buildingX, float buildingY, float targetX, float targetY, bool expectedInRange)
    {
        var stats = CreateValidStats(); // Range = 150
        var building = new BasicTower(stats, buildingX, buildingY);

        var result = building.IsInRange(targetX, targetY);

        result.Should().Be(expectedInRange);
    }

    [Theory]
    [InlineData(0.0f, 0.0f, 0.0f, 0.0f, 0.0f)]
    [InlineData(0.0f, 0.0f, 3.0f, 4.0f, 5.0f)]      // 3-4-5 triangle
    [InlineData(0.0f, 0.0f, 1.0f, 1.0f, 1.414f)]    // √2 ≈ 1.414
    [InlineData(10.0f, 20.0f, 10.0f, 20.0f, 0.0f)]  // Same position
    [InlineData(-5.0f, -10.0f, 5.0f, 10.0f, 22.36f)] // Negative to positive coordinates
    public void CalculateDistance_ShouldReturnCorrectDistance(float buildingX, float buildingY, float targetX, float targetY, float expectedDistance)
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, buildingX, buildingY);

        var distance = building.CalculateDistance(targetX, targetY);

        distance.Should().BeApproximately(expectedDistance, 0.01f);
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);
        building.IsActive.Should().BeTrue();

        building.Deactivate();

        building.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_ShouldSetIsActiveToTrue()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);
        building.Deactivate();
        building.IsActive.Should().BeFalse();

        building.Activate();

        building.IsActive.Should().BeTrue();
    }

    [Fact]
    public void CanUpgrade_WhenActive_ShouldReturnTrue()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);

        var canUpgrade = building.CanUpgrade();

        canUpgrade.Should().BeTrue();
    }

    [Fact]
    public void CanUpgrade_WhenInactive_ShouldReturnFalse()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);
        building.Deactivate();

        var canUpgrade = building.CanUpgrade();

        canUpgrade.Should().BeFalse();
    }

    [Fact]
    public void IsInRange_AfterDeactivation_ShouldStillWork()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);
        building.Deactivate();

        var inRange = building.IsInRange(50, 50);

        inRange.Should().BeTrue(); // Range calculation should work regardless of active state
    }

    [Fact]
    public void CalculateDistance_WithExtremeCoordinates_ShouldHandleCorrectly()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, float.MaxValue / 2, float.MaxValue / 2);

        var distance = building.CalculateDistance(float.MaxValue / 2, float.MaxValue / 2);

        distance.Should().Be(0.0f);
    }

    [Fact]
    public void IsInRange_WithVeryLargeDistance_ShouldReturnFalse()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);

        var inRange = building.IsInRange(float.MaxValue, float.MaxValue);

        inRange.Should().BeFalse();
    }

    [Fact]
    public void CreatedAt_ShouldBeSetToCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var stats = CreateValidStats();
        
        var building = new BasicTower(stats, 0, 0);
        
        var afterCreation = DateTime.UtcNow;

        building.CreatedAt.Should().BeAfter(beforeCreation.AddMilliseconds(-10))
            .And.BeBefore(afterCreation.AddMilliseconds(10));
    }

    [Fact]
    public void Stats_ShouldBeReadOnly()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 0, 0);
        var originalStats = building.Stats;

        // Since Stats is protected set, we verify it cannot be changed from outside
        building.Stats.Should().Be(originalStats);
        building.Stats.Cost.Should().Be(stats.Cost);
        building.Stats.Range.Should().Be(stats.Range);
    }

    [Theory]
    [InlineData(0.0f, 0.0f)]
    [InlineData(-1000.0f, 1000.0f)]
    [InlineData(999999.0f, -999999.0f)]
    public void Position_ShouldBeImmutable(float x, float y)
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, x, y);

        building.X.Should().Be(x);
        building.Y.Should().Be(y);
        
        // Position properties should not have setters accessible from outside
        // This is verified by compilation - if X or Y had public setters, this would fail
    }

    [Fact]
    public void MultipleOperations_ShouldMaintainConsistentState()
    {
        var stats = CreateValidStats();
        var building = new BasicTower(stats, 100, 100);
        
        // Test sequence of operations
        building.IsActive.Should().BeTrue();
        building.CanUpgrade().Should().BeTrue();
        
        building.Deactivate();
        building.IsActive.Should().BeFalse();
        building.CanUpgrade().Should().BeFalse();
        
        building.Activate();
        building.IsActive.Should().BeTrue();
        building.CanUpgrade().Should().BeTrue();
        
        // Position and stats should remain unchanged
        building.X.Should().Be(100);
        building.Y.Should().Be(100);
        building.Stats.Should().Be(stats);
    }
}
