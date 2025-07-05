using System;
using Xunit;
using FluentAssertions;
using Game.Domain.ValueObjects;

namespace Game.Tests.Domain.ValueObjects
{
    public class BuildingStatsTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateBuildingStats()
        {
            // Arrange & Act
            var stats = new BuildingStats(
                cost: 50,
                damage: 25,
                range: 200.0f,
                fireRate: 1.5f,
                bulletSpeed: 800.0f,
                shootSound: "test_shoot",
                impactSound: "test_impact",
                description: "Test building"
            );

            // Assert
            stats.Cost.Should().Be(50);
            stats.Damage.Should().Be(25);
            stats.Range.Should().Be(200.0f);
            stats.FireRate.Should().Be(1.5f);
            stats.BulletSpeed.Should().Be(800.0f);
            stats.ShootSound.Should().Be("test_shoot");
            stats.ImpactSound.Should().Be("test_impact");
            stats.Description.Should().Be("Test building");
        }

        [Theory]
        [InlineData(-1, 10, 100.0f, 1.0f, 500.0f)] // Negative cost
        [InlineData(10, -1, 100.0f, 1.0f, 500.0f)] // Negative damage
        [InlineData(10, 10, 0.0f, 1.0f, 500.0f)]  // Zero range
        [InlineData(10, 10, 100.0f, 0.0f, 500.0f)] // Zero fire rate
        [InlineData(10, 10, 100.0f, 1.0f, 0.0f)]  // Zero bullet speed
        public void Constructor_WithInvalidParameters_ShouldThrowArgumentException(
            int cost, int damage, float range, float fireRate, float bulletSpeed)
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() => new BuildingStats(
                cost, damage, range, fireRate, bulletSpeed, "", "", ""
            ));
        }

        [Fact]
        public void Constructor_WithNullStrings_ShouldUseEmptyStrings()
        {
            // Arrange & Act
            var stats = new BuildingStats(10, 10, 100.0f, 1.0f, 500.0f, null, null, null);

            // Assert
            stats.ShootSound.Should().Be(string.Empty);
            stats.ImpactSound.Should().Be(string.Empty);
            stats.Description.Should().Be(string.Empty);
        }

        [Fact]
        public void CreateDefault_ShouldReturnValidDefaultStats()
        {
            // Arrange & Act
            var stats = BuildingStats.CreateDefault();

            // Assert
            stats.Cost.Should().Be(10);
            stats.Damage.Should().Be(10);
            stats.Range.Should().Be(150.0f);
            stats.FireRate.Should().Be(1.0f);
            stats.BulletSpeed.Should().Be(900.0f);
            stats.ShootSound.Should().Be("basic_turret_shoot");
            stats.ImpactSound.Should().Be("basic_bullet_impact");
            stats.Description.Should().NotBeEmpty();
        }

        [Fact]
        public void DamagePerSecond_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new BuildingStats(10, 20, 100.0f, 2.0f, 500.0f, "", "", "");

            // Act
            var dps = stats.DamagePerSecond;

            // Assert
            dps.Should().Be(10.0f); // 20 damage / 2.0 fire rate = 10 DPS
        }

        [Fact]
        public void CostEffectiveness_ShouldCalculateCorrectly()
        {
            // Arrange
            var stats = new BuildingStats(25, 20, 100.0f, 2.0f, 500.0f, "", "", "");

            // Act
            var effectiveness = stats.CostEffectiveness;

            // Assert
            effectiveness.Should().Be(0.4f); // (20/2.0) / 25 = 10/25 = 0.4
        }

        [Fact]
        public void CostEffectiveness_WithZeroCost_ShouldReturnZero()
        {
            // Arrange
            var stats = new BuildingStats(0, 20, 100.0f, 2.0f, 500.0f, "", "", "");

            // Act
            var effectiveness = stats.CostEffectiveness;

            // Assert
            effectiveness.Should().Be(0.0f);
        }

        [Fact]
        public void Equals_WithIdenticalStats_ShouldReturnTrue()
        {
            // Arrange
            var stats1 = new BuildingStats(10, 20, 100.0f, 1.0f, 500.0f, "sound1", "sound2", "desc");
            var stats2 = new BuildingStats(10, 20, 100.0f, 1.0f, 500.0f, "sound1", "sound2", "desc");

            // Act & Assert
            stats1.Equals(stats2).Should().BeTrue();
            (stats1 == stats2).Should().BeTrue();
            (stats1 != stats2).Should().BeFalse();
        }

        [Fact]
        public void Equals_WithDifferentStats_ShouldReturnFalse()
        {
            // Arrange
            var stats1 = new BuildingStats(10, 20, 100.0f, 1.0f, 500.0f, "sound1", "sound2", "desc");
            var stats2 = new BuildingStats(15, 20, 100.0f, 1.0f, 500.0f, "sound1", "sound2", "desc");

            // Act & Assert
            stats1.Equals(stats2).Should().BeFalse();
            (stats1 == stats2).Should().BeFalse();
            (stats1 != stats2).Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_WithIdenticalStats_ShouldReturnSameHash()
        {
            // Arrange
            var stats1 = new BuildingStats(10, 20, 100.0f, 1.0f, 500.0f, "sound1", "sound2", "desc");
            var stats2 = new BuildingStats(10, 20, 100.0f, 1.0f, 500.0f, "sound1", "sound2", "desc");

            // Act & Assert
            stats1.GetHashCode().Should().Be(stats2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldContainRelevantInformation()
        {
            // Arrange
            var stats = new BuildingStats(50, 25, 200.0f, 1.25f, 600.0f, "", "", "");

            // Act
            var result = stats.ToString();

            // Assert
            result.Should().Contain("50"); // Cost
            result.Should().Contain("25"); // Damage
            result.Should().Contain("200"); // Range
            result.Should().Contain("1.25"); // Fire rate
            result.Should().Contain("20.0"); // DPS (25/1.25)
        }
    }
}
