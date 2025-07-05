using System;
using Xunit;
using FluentAssertions;
using Game.Domain.ValueObjects;

namespace Game.Tests.Domain.ValueObjects
{
    public class EnemyStatsTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateEnemyStats()
        {
            // Arrange & Act
            var stats = new EnemyStats(
                maxHealth: 150,
                speed: 80.0f,
                damage: 15,
                rewardGold: 10,
                rewardXp: 25,
                description: "Test enemy"
            );

            // Assert
            stats.MaxHealth.Should().Be(150);
            stats.Speed.Should().Be(80.0f);
            stats.Damage.Should().Be(15);
            stats.RewardGold.Should().Be(10);
            stats.RewardXp.Should().Be(25);
            stats.Description.Should().Be("Test enemy");
        }

        [Theory]
        [InlineData(0, 50.0f, 10, 5, 10)]  // Zero health
        [InlineData(100, 0.0f, 10, 5, 10)] // Zero speed
        [InlineData(100, 50.0f, -1, 5, 10)] // Negative damage
        [InlineData(100, 50.0f, 10, -1, 10)] // Negative gold
        [InlineData(100, 50.0f, 10, 5, -1)]  // Negative XP
        public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(
            int maxHealth, float speed, int damage, int rewardGold, int rewardXp)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new EnemyStats(
                maxHealth, speed, damage, rewardGold, rewardXp, ""
            ));
        }

        [Fact]
        public void Constructor_WithNullDescription_ShouldUseEmptyString()
        {
            // Arrange & Act
            var stats = new EnemyStats(100, 50.0f, 10, 5, 10, null);

            // Assert
            stats.Description.Should().Be(string.Empty);
        }

        [Fact]
        public void CreateDefault_ShouldReturnValidDefaultStats()
        {
            // Arrange & Act
            var stats = EnemyStats.CreateDefault();

            // Assert
            stats.MaxHealth.Should().Be(100);
            stats.Speed.Should().Be(60.0f);
            stats.Damage.Should().Be(10);
            stats.RewardGold.Should().Be(5);
            stats.RewardXp.Should().Be(10);
            stats.Description.Should().NotBeEmpty();
        }

        [Fact]
        public void WithMultipliers_WithValidMultipliers_ShouldReturnModifiedStats()
        {
            // Arrange
            var originalStats = new EnemyStats(100, 60.0f, 10, 5, 10, "test");

            // Act
            var modifiedStats = originalStats.WithMultipliers(1.5f, 2.0f);

            // Assert
            modifiedStats.MaxHealth.Should().Be(150); // 100 * 1.5
            modifiedStats.Speed.Should().Be(120.0f);  // 60 * 2.0
            modifiedStats.Damage.Should().Be(10);    // Unchanged
            modifiedStats.RewardGold.Should().Be(5); // Unchanged
            modifiedStats.RewardXp.Should().Be(10);  // Unchanged
            modifiedStats.Description.Should().Be("test"); // Unchanged
        }

        [Theory]
        [InlineData(0.0f, 1.0f)] // Zero health multiplier
        [InlineData(1.0f, 0.0f)] // Zero speed multiplier
        [InlineData(-1.0f, 1.0f)] // Negative health multiplier
        [InlineData(1.0f, -1.0f)] // Negative speed multiplier
        public void WithMultipliers_WithInvalidMultipliers_ShouldThrowArgumentException(
            float healthMultiplier, float speedMultiplier)
        {
            // Arrange
            var stats = EnemyStats.CreateDefault();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                stats.WithMultipliers(healthMultiplier, speedMultiplier));
        }

        [Fact]
        public void ThreatLevel_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new EnemyStats(200, 50.0f, 20, 10, 15, "");

            // Act
            var threatLevel = stats.ThreatLevel;

            // Assert
            // (200 * 20 * 50) / 1000 = 200000 / 1000 = 200
            threatLevel.Should().Be(200.0f);
        }

        [Fact]
        public void RewardEfficiency_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new EnemyStats(100, 50.0f, 20, 10, 20, "");
            // Threat level: (100 * 20 * 50) / 1000 = 100
            // Total rewards: 10 + 20 = 30
            // Efficiency: 30 / 100 = 0.3

            // Act
            var efficiency = stats.RewardEfficiency;

            // Assert
            efficiency.Should().Be(0.3f);
        }

        [Fact]
        public void RewardEfficiency_WithZeroThreatLevel_ShouldReturnZero()
        {
            // Arrange - stats that would result in zero threat (shouldn't be possible with valid constructor)
            // This test ensures the calculation handles edge cases safely
            var stats = new EnemyStats(1, 1.0f, 0, 10, 20, "");
            // Threat level: (1 * 0 * 1) / 1000 = 0

            // Act
            var efficiency = stats.RewardEfficiency;

            // Assert
            efficiency.Should().Be(0.0f);
        }

        [Fact]
        public void Equals_WithIdenticalStats_ShouldReturnTrue()
        {
            // Arrange
            var stats1 = new EnemyStats(100, 60.0f, 10, 5, 15, "test");
            var stats2 = new EnemyStats(100, 60.0f, 10, 5, 15, "test");

            // Act & Assert
            stats1.Equals(stats2).Should().BeTrue();
            (stats1 == stats2).Should().BeTrue();
            (stats1 != stats2).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentStats_ShouldReturnFalse()
        {
            // Arrange
            var stats1 = new EnemyStats(100, 60.0f, 10, 5, 15, "test");
            var stats2 = new EnemyStats(120, 60.0f, 10, 5, 15, "test");

            // Act & Assert
            stats1.Equals(stats2).Should().BeFalse();
            (stats1 == stats2).Should().BeFalse();
            (stats1 != stats2).Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_WithIdenticalStats_ShouldReturnSameHash()
        {
            // Arrange
            var stats1 = new EnemyStats(100, 60.0f, 10, 5, 15, "test");
            var stats2 = new EnemyStats(100, 60.0f, 10, 5, 15, "test");

            // Act & Assert
            stats1.GetHashCode().Should().Be(stats2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldContainRelevantInformation()
        {
            // Arrange
            var stats = new EnemyStats(150, 75.0f, 20, 12, 25, "");

            // Act
            var result = stats.ToString();

            // Assert
            result.Should().Contain("150"); // Health
            result.Should().Contain("75"); // Speed
            result.Should().Contain("20"); // Damage
            result.Should().Contain("12"); // Gold
            result.Should().Contain("25"); // XP
            result.Should().Contain("Threat"); // Should show threat level
        }
    }
}
