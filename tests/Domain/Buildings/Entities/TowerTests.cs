using System;
using FluentAssertions;
using Game.Domain.Buildings.Entities;
using Game.Domain.Buildings.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Buildings.Entities;

public class TowerTests
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
            description: "Test tower"
        );
    }

    [Fact]
    public void Constructor_ShouldInitializeLastShotTimeToZero()
    {
        var stats = CreateValidStats();
        
        var tower = new BasicTower(stats, 0, 0);

        tower.LastShotTime.Should().Be(0f);
    }

    [Theory]
    [InlineData(30.0f, 1.0f)]   // 30 attacks/30 = 1 second cooldown
    [InlineData(60.0f, 0.5f)]   // 60 attacks/30 = 0.5 second cooldown  
    [InlineData(15.0f, 2.0f)]   // 15 attacks/30 = 2 second cooldown
    [InlineData(90.0f, 0.333f)] // 90 attacks/30 = 0.333 second cooldown
    public void CanShoot_WithSufficientTime_ShouldReturnTrue(float attackSpeed, float expectedCooldown)
    {
        var stats = new BuildingStats(100, 25, 150.0f, attackSpeed, 500.0f, "", "", "");
        var tower = new BasicTower(stats, 0, 0);
        var currentTime = expectedCooldown + 0.1f; // Slightly more than cooldown

        var canShoot = tower.CanShoot(currentTime);

        canShoot.Should().BeTrue();
    }

    [Theory]
    [InlineData(30.0f, 0.5f)]   // 30 attacks/30 = 1 second cooldown, but only 0.5s passed
    [InlineData(60.0f, 0.2f)]   // 60 attacks/30 = 0.5 second cooldown, but only 0.2s passed
    [InlineData(15.0f, 1.0f)]   // 15 attacks/30 = 2 second cooldown, but only 1s passed
    public void CanShoot_WithInsufficientTime_ShouldReturnFalse(float attackSpeed, float timePassed)
    {
        var stats = new BuildingStats(100, 25, 150.0f, attackSpeed, 500.0f, "", "", "");
        var tower = new BasicTower(stats, 0, 0);
        var expectedCooldown = 30f / attackSpeed;
        var shotTime = expectedCooldown + 1.0f; // Ensure first shot is allowed
        tower.Shoot(shotTime); // Set an initial shot time
        var currentTime = shotTime + timePassed;

        var canShoot = tower.CanShoot(currentTime);

        canShoot.Should().BeFalse();
    }

    [Fact]
    public void CanShoot_WhenInactive_ShouldReturnFalse()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);
        tower.Deactivate();

        var canShoot = tower.CanShoot(100.0f); // Long time passed

        canShoot.Should().BeFalse();
    }

    [Fact]
    public void CanShoot_WhenActiveWithSufficientTime_ShouldReturnTrue()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);
        
        var canShoot = tower.CanShoot(10.0f); // Fresh tower, plenty of time

        canShoot.Should().BeTrue();
    }

    [Fact]
    public void Shoot_WhenCanShoot_ShouldUpdateLastShotTime()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);
        var shotTime = 5.0f;

        tower.Shoot(shotTime);

        tower.LastShotTime.Should().Be(shotTime);
    }

    [Fact]
    public void Shoot_WhenCannotShoot_ShouldThrowInvalidOperationException()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);
        tower.Shoot(1.0f); // First shot
        var secondShotTime = 1.1f; // Too soon after first shot

        var action = () => tower.Shoot(secondShotTime);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Tower cannot shoot at this time");
    }

    [Fact]
    public void Shoot_WhenInactive_ShouldThrowInvalidOperationException()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);
        tower.Deactivate();

        var action = () => tower.Shoot(10.0f);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Tower cannot shoot at this time");
    }

    [Fact]
    public void ShootingSequence_ShouldRespectAttackSpeed()
    {
        var attackSpeed = 60.0f; // 60 attacks per 30 seconds = 0.5 second cooldown
        var stats = new BuildingStats(100, 25, 150.0f, attackSpeed, 500.0f, "", "", "");
        var tower = new BasicTower(stats, 0, 0);

        // First shot should be allowed with sufficient time from start
        tower.CanShoot(1.0f).Should().BeTrue();
        tower.Shoot(1.0f);
        tower.LastShotTime.Should().Be(1.0f);

        // Second shot too soon should fail
        tower.CanShoot(1.3f).Should().BeFalse();

        // Second shot after sufficient time should succeed
        tower.CanShoot(1.6f).Should().BeTrue();
        tower.Shoot(1.6f);
        tower.LastShotTime.Should().Be(1.6f);
    }

    [Fact]
    public void MultipleShoots_ShouldUpdateLastShotTimeCorrectly()
    {
        var stats = CreateValidStats(); // Attack speed 30 = 1 second cooldown
        var tower = new BasicTower(stats, 0, 0);

        tower.Shoot(1.0f);
        tower.LastShotTime.Should().Be(1.0f);

        tower.Shoot(3.0f); // 2 seconds later, should be allowed
        tower.LastShotTime.Should().Be(3.0f);

        tower.Shoot(5.0f); // 2 seconds later, should be allowed
        tower.LastShotTime.Should().Be(5.0f);
    }

    [Theory]
    [InlineData(1.0f, 1.0f)]     // Shoot immediately twice (same time)
    [InlineData(1.0f, 1.5f)]     // Second shot too soon
    [InlineData(2.0f, 2.8f)]     // Second shot still too soon
    public void ConsecutiveShoots_WithInsufficientTime_ShouldFail(float firstShotTime, float secondShotTime)
    {
        var stats = CreateValidStats(); // Attack speed 30 = 1 second cooldown
        var tower = new BasicTower(stats, 0, 0);

        tower.Shoot(firstShotTime);
        
        var action = () => tower.Shoot(secondShotTime);

        action.Should().Throw<InvalidOperationException>();
        tower.LastShotTime.Should().Be(firstShotTime); // Should not change
    }

    [Fact]
    public void CanShoot_AtExactCooldownTime_ShouldReturnTrue()
    {
        var stats = CreateValidStats(); // Attack speed 30 = 1 second cooldown
        var tower = new BasicTower(stats, 0, 0);
        tower.Shoot(1.0f);

        var canShoot = tower.CanShoot(2.0f); // Exactly 1 second later

        canShoot.Should().BeTrue();
    }

    [Fact]
    public void DeactivateAndReactivate_ShouldNotAffectLastShotTime()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);
        tower.Shoot(1.0f);

        tower.Deactivate();
        tower.Activate();

        tower.LastShotTime.Should().Be(1.0f);
        tower.CanShoot(3.0f).Should().BeTrue(); // Should still respect cooldown
    }

    [Fact]
    public void Tower_ShouldInheritBuildingBehavior()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 50, 75);

        // Verify it inherits Building behavior
        tower.Id.Should().NotBeEmpty();
        tower.X.Should().Be(50);
        tower.Y.Should().Be(75);
        tower.IsActive.Should().BeTrue();
        tower.IsInRange(60, 80).Should().BeTrue();
    }

    [Theory]
    [InlineData(1.0f)]
    [InlineData(10.0f)]
    [InlineData(100.0f)]
    public void AttackSpeedCalculation_ShouldBeConsistentWithFormula(float attackSpeed)
    {
        var stats = new BuildingStats(100, 25, 150.0f, attackSpeed, 500.0f, "", "", "");
        var tower = new BasicTower(stats, 0, 0);
        var expectedCooldown = 30f / attackSpeed;
        var shotTime = expectedCooldown + 1.0f; // Use a time that allows the first shot

        tower.Shoot(shotTime);
        
        // Should not be able to shoot before cooldown
        tower.CanShoot(shotTime + expectedCooldown - 0.001f).Should().BeFalse();
        
        // Should be able to shoot after cooldown
        tower.CanShoot(shotTime + expectedCooldown + 0.001f).Should().BeTrue();
    }

    [Fact]
    public void LastShotTime_ShouldBeReadOnly()
    {
        var stats = CreateValidStats();
        var tower = new BasicTower(stats, 0, 0);

        // LastShotTime should only be modifiable through Shoot method
        // This is verified by the fact that there's no public setter
        tower.LastShotTime.Should().Be(0f);
        
        tower.Shoot(5.0f);
        tower.LastShotTime.Should().Be(5.0f);
    }
}
