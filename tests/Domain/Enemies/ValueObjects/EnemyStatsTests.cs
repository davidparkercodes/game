using System;
using FluentAssertions;
using Game.Domain.Enemies.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Enemies.ValueObjects;

public class EnemyStatsTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEnemyStats()
    {
        var maxHealth = 150;
        var speed = 80.0f;
        var damage = 25;
        var rewardGold = 10;
        var rewardXp = 20;
        var description = "Fast enemy";

        var stats = new EnemyStats(maxHealth, speed, damage, rewardGold, rewardXp, description);

        stats.MaxHealth.Should().Be(maxHealth);
        stats.Speed.Should().Be(speed);
        stats.Damage.Should().Be(damage);
        stats.RewardGold.Should().Be(rewardGold);
        stats.RewardXp.Should().Be(rewardXp);
        stats.Description.Should().Be(description);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithNonPositiveHealth_ShouldThrowArgumentException(int maxHealth)
    {
        var action = () => new EnemyStats(maxHealth, 60.0f, 10, 5, 10, "test");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Max health must be positive*")
            .And.ParamName.Should().Be("maxHealth");
    }

    [Theory]
    [InlineData(0.0f)]
    [InlineData(-1.0f)]
    [InlineData(-100.0f)]
    public void Constructor_WithNonPositiveSpeed_ShouldThrowArgumentException(float speed)
    {
        var action = () => new EnemyStats(100, speed, 10, 5, 10, "test");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Speed must be positive*")
            .And.ParamName.Should().Be("speed");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void Constructor_WithNegativeDamage_ShouldThrowArgumentException(int damage)
    {
        var action = () => new EnemyStats(100, 60.0f, damage, 5, 10, "test");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Damage cannot be negative*")
            .And.ParamName.Should().Be("damage");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void Constructor_WithNegativeRewardGold_ShouldThrowArgumentException(int rewardGold)
    {
        var action = () => new EnemyStats(100, 60.0f, 10, rewardGold, 10, "test");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Reward gold cannot be negative*")
            .And.ParamName.Should().Be("rewardGold");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void Constructor_WithNegativeRewardXp_ShouldThrowArgumentException(int rewardXp)
    {
        var action = () => new EnemyStats(100, 60.0f, 10, 5, rewardXp, "test");

        action.Should().Throw<ArgumentException>()
            .WithMessage("Reward XP cannot be negative*")
            .And.ParamName.Should().Be("rewardXp");
    }

    [Fact]
    public void Constructor_WithNullDescription_ShouldUseEmptyString()
    {
        var stats = new EnemyStats(100, 60.0f, 10, 5, 10, null!);

        stats.Description.Should().Be(string.Empty);
    }

    [Fact]
    public void Constructor_WithZeroDamage_ShouldBeAllowed()
    {
        var stats = new EnemyStats(100, 60.0f, 0, 5, 10, "peaceful");

        stats.Damage.Should().Be(0);
    }

    [Fact]
    public void Constructor_WithZeroRewards_ShouldBeAllowed()
    {
        var stats = new EnemyStats(100, 60.0f, 10, 0, 0, "no reward");

        stats.RewardGold.Should().Be(0);
        stats.RewardXp.Should().Be(0);
    }

    // REMOVED: CreateDefault test - EnemyStats no longer has hardcoded CreateDefault method
    // All EnemyStats should be created from config data via services

    [Theory]
    [InlineData(2.0f, 1.5f)]
    [InlineData(0.5f, 2.0f)]
    [InlineData(1.0f, 1.0f)]
    [InlineData(3.0f, 0.8f)]
    public void WithMultipliers_WithValidMultipliers_ShouldApplyCorrectly(float healthMultiplier, float speedMultiplier)
    {
        var originalStats = new EnemyStats(100, 60.0f, 15, 5, 10, "test");

        var modifiedStats = originalStats.WithMultipliers(healthMultiplier, speedMultiplier);

        modifiedStats.MaxHealth.Should().Be((int)(100 * healthMultiplier));
        modifiedStats.Speed.Should().BeApproximately(60.0f * speedMultiplier, 0.01f);
        modifiedStats.Damage.Should().Be(15); // Should remain unchanged
        modifiedStats.RewardGold.Should().Be(5); // Should remain unchanged
        modifiedStats.RewardXp.Should().Be(10); // Should remain unchanged
        modifiedStats.Description.Should().Be("test"); // Should remain unchanged
    }

    [Theory]
    [InlineData(0.0f, 1.0f)]
    [InlineData(-1.0f, 1.0f)]
    [InlineData(1.0f, 0.0f)]
    [InlineData(1.0f, -1.0f)]
    public void WithMultipliers_WithInvalidMultipliers_ShouldThrowArgumentException(float healthMultiplier, float speedMultiplier)
    {
        var stats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        var action = () => stats.WithMultipliers(healthMultiplier, speedMultiplier);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(100, 10, 60.0f, 60.0f)]     // (100 * 10 * 60) / 1000 = 60.0
    [InlineData(200, 15, 30.0f, 90.0f)]     // (200 * 15 * 30) / 1000 = 90.0
    [InlineData(50, 5, 120.0f, 30.0f)]      // (50 * 5 * 120) / 1000 = 30.0
    [InlineData(1, 1, 1000.0f, 1.0f)]       // (1 * 1 * 1000) / 1000 = 1.0
    public void ThreatLevel_ShouldCalculateCorrectly(int health, int damage, float speed, float expectedThreat)
    {
        var stats = new EnemyStats(health, speed, damage, 5, 10, "test");

        stats.ThreatLevel.Should().BeApproximately(expectedThreat, 0.01f);
    }

    [Theory]
    [InlineData(100, 10, 60.0f, 5, 10, 0.25f)]   // Threat = 60.0, Rewards = 15, Efficiency = 15/60 = 0.25
    [InlineData(200, 15, 30.0f, 10, 20, 0.33f)]  // Threat = 90.0, Rewards = 30, Efficiency = 30/90 = 0.33
    [InlineData(1, 0, 1.0f, 100, 100, 200.0f)]   // Threat = 0.0, Rewards = 200, Efficiency = 200/0 = inf
    public void RewardEfficiency_ShouldCalculateCorrectly(int health, int damage, float speed, int gold, int xp, float expectedEfficiency)
    {
        var stats = new EnemyStats(health, speed, damage, gold, xp, "test");

        if (expectedEfficiency == 200.0f) // Special case for zero threat
        {
            // When threat is 0, efficiency should be 0 (as per the property logic)
            stats.RewardEfficiency.Should().Be(0);
        }
        else
        {
            stats.RewardEfficiency.Should().BeApproximately(expectedEfficiency, 0.01f);
        }
    }

    [Fact]
    public void RewardEfficiency_WithZeroThreat_ShouldReturnZero()
    {
        var stats = new EnemyStats(1, 1.0f, 0, 100, 100, "test"); // Zero damage = zero threat

        stats.RewardEfficiency.Should().Be(0);
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var stats = new EnemyStats(150, 75.5f, 20, 8, 15, "test");

        var result = stats.ToString();

        result.Should().Contain("HP:150")
            .And.Contain("Speed:75.5")
            .And.Contain("Damage:20")
            .And.Contain("Gold:8")
            .And.Contain("XP:15")
            .And.Contain("Threat:");
    }

    [Fact]
    public void Equals_WithIdenticalStats_ShouldReturnTrue()
    {
        var stats1 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");
        var stats2 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        stats1.Equals(stats2).Should().BeTrue();
        (stats1 == stats2).Should().BeTrue();
        (stats1 != stats2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentHealth_ShouldReturnFalse()
    {
        var stats1 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");
        var stats2 = new EnemyStats(150, 60.0f, 10, 5, 10, "test");

        stats1.Equals(stats2).Should().BeFalse();
        (stats1 == stats2).Should().BeFalse();
        (stats1 != stats2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSlightlyDifferentSpeed_ShouldReturnTrue()
    {
        var stats1 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");
        var stats2 = new EnemyStats(100, 60.0005f, 10, 5, 10, "test"); // Within tolerance

        stats1.Equals(stats2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithSignificantlyDifferentSpeed_ShouldReturnFalse()
    {
        var stats1 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");
        var stats2 = new EnemyStats(100, 61.0f, 10, 5, 10, "test"); // Outside tolerance

        stats1.Equals(stats2).Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithIdenticalStats_ShouldReturnSameHashCode()
    {
        var stats1 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");
        var stats2 = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        stats1.GetHashCode().Should().Be(stats2.GetHashCode());
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        var stats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        stats.Equals(null).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        var stats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        stats.Equals("not enemy stats").Should().BeFalse();
    }

    [Fact]
    public void WithMultipliers_ShouldRoundHealthToInteger()
    {
        var stats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        var modifiedStats = stats.WithMultipliers(1.7f, 1.0f); // 100 * 1.7 = 170

        modifiedStats.MaxHealth.Should().Be(170);
    }

    [Fact]
    public void WithMultipliers_WithFractionalHealth_ShouldTruncate()
    {
        var stats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

        var modifiedStats = stats.WithMultipliers(1.99f, 1.0f); // 100 * 1.99 = 199.0 -> 199

        modifiedStats.MaxHealth.Should().Be(199);
    }

    [Theory]
    [InlineData(float.PositiveInfinity, 1.0f)]
    [InlineData(float.NaN, 1.0f)]
    [InlineData(1.0f, float.PositiveInfinity)]
    [InlineData(1.0f, float.NaN)]
    public void Constructor_WithSpecialFloatValues_ShouldBeAllowed(float speed, float multiplier)
    {
        if (float.IsInfinity(speed) || float.IsNaN(speed))
        {
            var action = () => new EnemyStats(100, speed, 10, 5, 10, "test");
            action.Should().NotThrow(); // Constructor should accept these values
        }
        else
        {
            var stats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");
            var action = () => stats.WithMultipliers(1.0f, multiplier);
            action.Should().NotThrow(); // WithMultipliers should accept these values
        }
    }
}
