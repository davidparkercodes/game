using System;
using FluentAssertions;
using Game.Domain.Buildings.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Buildings.ValueObjects;

public class BuildingStatsTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBuildingStats()
    {
        var cost = 100;
        var damage = 50;
        var range = 200.0f;
        var attackSpeed = 60.0f;
        var bulletSpeed = 1000.0f;
        var shootSound = "shoot.wav";
        var impactSound = "impact.wav";
        var description = "Test building";

        var stats = new BuildingStats(
            cost, damage, range, attackSpeed, bulletSpeed,
            shootSound, impactSound, description);

        stats.Cost.Should().Be(cost);
        stats.Damage.Should().Be(damage);
        stats.Range.Should().Be(range);
        stats.AttackSpeed.Should().Be(attackSpeed);
        stats.BulletSpeed.Should().Be(bulletSpeed);
        stats.ShootSound.Should().Be(shootSound);
        stats.ImpactSound.Should().Be(impactSound);
        stats.Description.Should().Be(description);
    }

    [Fact]
    public void Constructor_WithNegativeCost_ShouldThrowArgumentException()
    {
        var action = () => new BuildingStats(
            cost: -1, damage: 10, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Cost cannot be negative*")
            .And.ParamName.Should().Be("cost");
    }

    [Fact]
    public void Constructor_WithNegativeDamage_ShouldThrowArgumentException()
    {
        var action = () => new BuildingStats(
            cost: 10, damage: -1, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Damage cannot be negative*")
            .And.ParamName.Should().Be("damage");
    }

    [Fact]
    public void Constructor_WithZeroRange_ShouldThrowArgumentException()
    {
        var action = () => new BuildingStats(
            cost: 10, damage: 10, range: 0.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Range must be positive*")
            .And.ParamName.Should().Be("range");
    }

    [Fact]
    public void Constructor_WithNegativeRange_ShouldThrowArgumentException()
    {
        var action = () => new BuildingStats(
            cost: 10, damage: 10, range: -100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Range must be positive*")
            .And.ParamName.Should().Be("range");
    }

    [Fact]
    public void Constructor_WithZeroAttackSpeed_ShouldThrowArgumentException()
    {
        var action = () => new BuildingStats(
            cost: 10, damage: 10, range: 100.0f, attackSpeed: 0.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Attack speed must be positive*")
            .And.ParamName.Should().Be("attackSpeed");
    }

    [Fact]
    public void Constructor_WithZeroBulletSpeed_ShouldThrowArgumentException()
    {
        var action = () => new BuildingStats(
            cost: 10, damage: 10, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 0.0f, shootSound: "", impactSound: "", description: "");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Bullet speed must be positive*")
            .And.ParamName.Should().Be("bulletSpeed");
    }

    [Fact]
    public void Constructor_WithNullStrings_ShouldUseEmptyStrings()
    {
        var stats = new BuildingStats(
            cost: 10, damage: 10, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: null!, impactSound: null!, description: null!);

        stats.ShootSound.Should().Be(string.Empty);
        stats.ImpactSound.Should().Be(string.Empty);
        stats.Description.Should().Be(string.Empty);
    }

    // REMOVED: CreateDefault test - BuildingStats no longer has hardcoded CreateDefault method
    // All BuildingStats should be created from config data via services

    [Theory]
    [InlineData(10, 30.0f, 10.0f)]   // 10 damage at 30 attacks/30 = 10 DPS
    [InlineData(20, 60.0f, 40.0f)]   // 20 damage at 60 attacks/30 = 40 DPS  
    [InlineData(5, 15.0f, 2.5f)]     // 5 damage at 15 attacks/30 = 2.5 DPS
    public void DamagePerSecond_ShouldCalculateCorrectly(int damage, float attackSpeed, float expectedDps)
    {
        var stats = new BuildingStats(
            cost: 10, damage: damage, range: 100.0f, attackSpeed: attackSpeed,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        stats.DamagePerSecond.Should().BeApproximately(expectedDps, 0.01f);
    }

    [Theory]
    [InlineData(10, 10.0f, 1.0f)]    // 10 DPS for 10 cost = 1.0 effectiveness
    [InlineData(20, 10.0f, 0.5f)]    // 10 DPS for 20 cost = 0.5 effectiveness
    [InlineData(50, 25.0f, 0.5f)]    // 25 DPS for 50 cost = 0.5 effectiveness
    public void CostEffectiveness_ShouldCalculateCorrectly(int cost, float dps, float expectedEffectiveness)
    {
        var damage = (int)(dps * 30.0f / 30.0f); // Reverse engineer damage from DPS
        var stats = new BuildingStats(
            cost: cost, damage: damage, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        stats.CostEffectiveness.Should().BeApproximately(expectedEffectiveness, 0.01f);
    }

    [Fact]
    public void CostEffectiveness_WithZeroCost_ShouldReturnZero()
    {
        var stats = new BuildingStats(
            cost: 0, damage: 10, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        stats.CostEffectiveness.Should().Be(0);
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var stats = new BuildingStats(
            cost: 100, damage: 25, range: 150.5f, attackSpeed: 45.0f,
            bulletSpeed: 750.0f, shootSound: "", impactSound: "", description: "");

        var result = stats.ToString();

        result.Should().Contain("Cost:100")
            .And.Contain("Damage:25")
            .And.Contain("Range:150.5")
            .And.Contain("AttackSpeed:45.0")
            .And.Contain("DPS:");
    }

    [Fact]
    public void Equals_WithIdenticalStats_ShouldReturnTrue()
    {
        var stats1 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        var stats2 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        stats1.Equals(stats2).Should().BeTrue();
        (stats1 == stats2).Should().BeTrue();
        (stats1 != stats2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentCost_ShouldReturnFalse()
    {
        var stats1 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        var stats2 = new BuildingStats(
            cost: 15, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        stats1.Equals(stats2).Should().BeFalse();
        (stats1 == stats2).Should().BeFalse();
        (stats1 != stats2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSlightlyDifferentFloatingPointValues_ShouldReturnTrue()
    {
        var stats1 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        var stats2 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0005f, attackSpeed: 30.0001f,
            bulletSpeed: 500.0001f, shootSound: "shoot", impactSound: "impact", description: "test");

        stats1.Equals(stats2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSignificantlyDifferentFloatingPointValues_ShouldReturnFalse()
    {
        var stats1 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        var stats2 = new BuildingStats(
            cost: 10, damage: 20, range: 101.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        stats1.Equals(stats2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithIdenticalStats_ShouldReturnSameHashCode()
    {
        var stats1 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        var stats2 = new BuildingStats(
            cost: 10, damage: 20, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "shoot", impactSound: "impact", description: "test");

        stats1.GetHashCode().Should().Be(stats2.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var stats = new BuildingStats(
            cost: 10, damage: 10, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        stats.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var stats = new BuildingStats(
            cost: 10, damage: 10, range: 100.0f, attackSpeed: 30.0f,
            bulletSpeed: 500.0f, shootSound: "", impactSound: "", description: "");

        stats.Equals("not a BuildingStats").Should().BeFalse();
    }
}
