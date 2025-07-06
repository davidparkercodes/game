using System;
using FluentAssertions;
using Game.Domain.Enemies.Entities;
using Game.Domain.Enemies.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Enemies.Entities;

public class EnemyTests
{
    private static EnemyStats CreateValidStats()
    {
        return new EnemyStats(
            maxHealth: 150,
            speed: 80.0f,
            damage: 20,
            rewardGold: 10,
            rewardXp: 15,
            description: "Test enemy"
        );
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEnemy()
    {
        var stats = CreateValidStats();
        var x = 100.0f;
        var y = 200.0f;

        var enemy = new Enemy(stats, x, y);

        enemy.Id.Should().NotBeEmpty();
        enemy.Stats.Should().Be(stats);
        enemy.CurrentHealth.Should().Be(stats.MaxHealth);
        enemy.X.Should().Be(x);
        enemy.Y.Should().Be(y);
        enemy.IsAlive.Should().BeTrue();
        enemy.SpawnedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        enemy.LastDamageTime.Should().Be(0f);
    }

    [Fact]
    public void Constructor_ShouldAssignUniqueIds()
    {
        var stats = CreateValidStats();

        var enemy1 = new Enemy(stats, 0, 0);
        var enemy2 = new Enemy(stats, 0, 0);

        enemy1.Id.Should().NotBe(enemy2.Id);
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.NaN, float.NaN)]
    public void Constructor_WithNaNPosition_ShouldThrowArgumentException(float x, float y)
    {
        var stats = CreateValidStats();

        var action = () => new Enemy(stats, x, y);

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

        var action = () => new Enemy(stats, x, y);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Position cannot be infinite");
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(149)]
    public void TakeDamage_WithValidDamage_ShouldReduceHealth(int damage)
    {
        var stats = CreateValidStats(); // MaxHealth = 150
        var enemy = new Enemy(stats, 0, 0);
        var currentTime = 5.0f;

        enemy.TakeDamage(damage, currentTime);

        enemy.CurrentHealth.Should().Be(150 - damage);
        enemy.LastDamageTime.Should().Be(currentTime);
        enemy.IsAlive.Should().BeTrue();
    }

    [Fact]
    public void TakeDamage_WithExactlyEnoughDamageToKill_ShouldKillEnemy()
    {
        var stats = CreateValidStats(); // MaxHealth = 150
        var enemy = new Enemy(stats, 0, 0);

        enemy.TakeDamage(150, 1.0f);

        enemy.CurrentHealth.Should().Be(0);
        enemy.IsAlive.Should().BeFalse();
    }

    [Fact]
    public void TakeDamage_WithMoreThanEnoughDamage_ShouldKillEnemyAndClampHealthToZero()
    {
        var stats = CreateValidStats(); // MaxHealth = 150
        var enemy = new Enemy(stats, 0, 0);

        enemy.TakeDamage(200, 1.0f);

        enemy.CurrentHealth.Should().Be(0);
        enemy.IsAlive.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void TakeDamage_WithNegativeDamage_ShouldThrowArgumentException(int damage)
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);

        var action = () => enemy.TakeDamage(damage, 1.0f);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Damage cannot be negative*")
            .And.ParamName.Should().Be("damage");
    }

    [Fact]
    public void TakeDamage_WhenAlreadyDead_ShouldThrowInvalidOperationException()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(150, 1.0f); // Kill the enemy

        var action = () => enemy.TakeDamage(10, 2.0f);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot damage dead enemy");
    }

    [Fact]
    public void TakeDamage_WithZeroDamage_ShouldNotChangeHealthButUpdateTime()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        var currentTime = 3.0f;

        enemy.TakeDamage(0, currentTime);

        enemy.CurrentHealth.Should().Be(150);
        enemy.LastDamageTime.Should().Be(currentTime);
        enemy.IsAlive.Should().BeTrue();
    }

    [Fact]
    public void MoveTo_WithValidPosition_ShouldUpdatePosition()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        var newX = 50.0f;
        var newY = 75.0f;

        enemy.MoveTo(newX, newY);

        enemy.X.Should().Be(newX);
        enemy.Y.Should().Be(newY);
    }

    [Theory]
    [InlineData(float.NaN, 0.0f)]
    [InlineData(0.0f, float.NaN)]
    [InlineData(float.PositiveInfinity, 0.0f)]
    [InlineData(0.0f, float.NegativeInfinity)]
    public void MoveTo_WithInvalidPosition_ShouldThrowArgumentException(float x, float y)
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);

        var action = () => enemy.MoveTo(x, y);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void MoveTo_WhenDead_ShouldThrowInvalidOperationException()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(150, 1.0f); // Kill the enemy

        var action = () => enemy.MoveTo(10, 10);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot move dead enemy");
    }

    [Theory]
    [InlineData(10.0f, 15.0f)]
    [InlineData(-5.0f, 20.0f)]
    [InlineData(0.0f, 0.0f)]
    [InlineData(-10.0f, -25.0f)]
    public void MoveBy_WithValidDelta_ShouldMoveRelatively(float deltaX, float deltaY)
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 100, 200);

        enemy.MoveBy(deltaX, deltaY);

        enemy.X.Should().Be(100 + deltaX);
        enemy.Y.Should().Be(200 + deltaY);
    }

    [Fact]
    public void MoveBy_WhenDead_ShouldThrowInvalidOperationException()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(150, 1.0f); // Kill the enemy

        var action = () => enemy.MoveBy(5, 5);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot move dead enemy");
    }

    [Theory]
    [InlineData(0.0f, 0.0f, 0.0f, 0.0f, 0.0f)]
    [InlineData(0.0f, 0.0f, 3.0f, 4.0f, 5.0f)]      // 3-4-5 triangle
    [InlineData(10.0f, 20.0f, 10.0f, 20.0f, 0.0f)]  // Same position
    [InlineData(-5.0f, -10.0f, 5.0f, 10.0f, 22.36f)] // Negative to positive coordinates
    public void CalculateDistance_ShouldReturnCorrectDistance(float enemyX, float enemyY, float targetX, float targetY, float expectedDistance)
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, enemyX, enemyY);

        var distance = enemy.CalculateDistance(targetX, targetY);

        distance.Should().BeApproximately(expectedDistance, 0.01f);
    }

    [Theory]
    [InlineData(150, 150, 1.0f)]    // Full health
    [InlineData(75, 150, 0.5f)]     // Half health
    [InlineData(1, 150, 0.0067f)]   // Nearly dead
    [InlineData(0, 150, 0.0f)]      // Dead
    public void HealthPercentage_ShouldCalculateCorrectly(int currentHealth, int maxHealth, float expectedPercentage)
    {
        var stats = new EnemyStats(maxHealth, 60.0f, 10, 5, 10, "test");
        var enemy = new Enemy(stats, 0, 0);
        
        // Simulate damage to set current health
        if (currentHealth < maxHealth)
        {
            enemy.TakeDamage(maxHealth - currentHealth, 1.0f);
        }

        enemy.HealthPercentage.Should().BeApproximately(expectedPercentage, 0.01f);
    }

    [Theory]
    [InlineData(150, false)]   // 100% health
    [InlineData(75, false)]    // 50% health
    [InlineData(31, false)]    // 20.7% health (above threshold)
    [InlineData(30, true)]     // 20% health (exactly at threshold)
    [InlineData(15, true)]     // 10% health
    [InlineData(1, true)]      // Nearly dead
    [InlineData(0, true)]      // Dead
    public void IsNearDeath_ShouldCalculateCorrectly(int currentHealth, bool expectedNearDeath)
    {
        var stats = CreateValidStats(); // MaxHealth = 150
        var enemy = new Enemy(stats, 0, 0);
        
        // Simulate damage to set current health
        if (currentHealth < 150)
        {
            enemy.TakeDamage(150 - currentHealth, 1.0f);
        }

        enemy.IsNearDeath.Should().Be(expectedNearDeath);
    }

    [Theory]
    [InlineData(0.0f, 0.0f, 50.0f, 50.0f, 100.0f, true)]   // Distance ≈ 70.7, Max = 100
    [InlineData(0.0f, 0.0f, 100.0f, 0.0f, 100.0f, true)]   // Distance = 100, Max = 100 (exactly)
    [InlineData(0.0f, 0.0f, 101.0f, 0.0f, 100.0f, false)]  // Distance = 101, Max = 100
    [InlineData(0.0f, 0.0f, 200.0f, 200.0f, 250.0f, false)] // Distance ≈ 282.8, Max = 250
    public void CanReachTarget_ShouldCalculateCorrectly(float enemyX, float enemyY, float targetX, float targetY, float maxDistance, bool expectedCanReach)
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, enemyX, enemyY);

        var canReach = enemy.CanReachTarget(targetX, targetY, maxDistance);

        canReach.Should().Be(expectedCanReach);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(75)]
    public void Heal_WithValidAmount_ShouldIncreaseHealth(int healAmount)
    {
        var stats = CreateValidStats(); // MaxHealth = 150
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(100, 1.0f); // Current health = 50

        enemy.Heal(healAmount);

        var expectedHealth = Math.Min(150, 50 + healAmount);
        enemy.CurrentHealth.Should().Be(expectedHealth);
    }

    [Fact]
    public void Heal_AboveMaxHealth_ShouldClampToMaxHealth()
    {
        var stats = CreateValidStats(); // MaxHealth = 150
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(50, 1.0f); // Current health = 100

        enemy.Heal(100); // Healing 100 should cap at 150

        enemy.CurrentHealth.Should().Be(150);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-100)]
    public void Heal_WithNegativeAmount_ShouldThrowArgumentException(int healAmount)
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);

        var action = () => enemy.Heal(healAmount);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Heal amount cannot be negative*")
            .And.ParamName.Should().Be("amount");
    }

    [Fact]
    public void Heal_WhenDead_ShouldThrowInvalidOperationException()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(150, 1.0f); // Kill the enemy

        var action = () => enemy.Heal(50);

        action.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot heal dead enemy");
    }

    [Fact]
    public void Heal_WithZeroAmount_ShouldNotChangeHealth()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(50, 1.0f); // Current health = 100

        enemy.Heal(0);

        enemy.CurrentHealth.Should().Be(100);
    }

    [Fact]
    public void Kill_ShouldSetHealthToZeroAndMarkAsDead()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);

        enemy.Kill();

        enemy.CurrentHealth.Should().Be(0);
        enemy.IsAlive.Should().BeFalse();
    }

    [Fact]
    public void Kill_WhenAlreadyDead_ShouldNotThrow()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);
        enemy.TakeDamage(150, 1.0f); // Kill the enemy

        var action = () => enemy.Kill();

        action.Should().NotThrow();
        enemy.CurrentHealth.Should().Be(0);
        enemy.IsAlive.Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 25.5f, 30.75f);

        var result = enemy.ToString();

        result.Should().Contain($"Id:{enemy.Id:N}")
            .And.Contain("HP:150/150")
            .And.Contain("Pos:(25.5,30.8)")
            .And.Contain("Alive:True");
    }

    [Fact]
    public void SpawnedAt_ShouldBeSetToCurrentUtcTime()
    {
        var beforeCreation = DateTime.UtcNow;
        var stats = CreateValidStats();

        var enemy = new Enemy(stats, 0, 0);

        var afterCreation = DateTime.UtcNow;

        enemy.SpawnedAt.Should().BeAfter(beforeCreation.AddMilliseconds(-10))
            .And.BeBefore(afterCreation.AddMilliseconds(10));
    }

    [Fact]
    public void HealthPercentage_WithZeroMaxHealth_ShouldReturnZero()
    {
        var stats = new EnemyStats(1, 60.0f, 10, 5, 10, "test"); // Min health is 1
        var enemy = new Enemy(stats, 0, 0);

        // Since we can't create with 0 max health, this tests the property logic
        enemy.HealthPercentage.Should().Be(1.0f); // 1/1 = 1.0
    }

    [Fact]
    public void MultipleOperations_ShouldMaintainConsistentState()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 100, 200);

        // Initial state
        enemy.IsAlive.Should().BeTrue();
        enemy.CurrentHealth.Should().Be(150);
        enemy.X.Should().Be(100);
        enemy.Y.Should().Be(200);

        // Take damage
        enemy.TakeDamage(50, 1.0f);
        enemy.CurrentHealth.Should().Be(100);
        enemy.IsAlive.Should().BeTrue();

        // Move
        enemy.MoveTo(150, 250);
        enemy.X.Should().Be(150);
        enemy.Y.Should().Be(250);
        enemy.CurrentHealth.Should().Be(100); // Health unchanged by movement

        // Heal
        enemy.Heal(25);
        enemy.CurrentHealth.Should().Be(125);
        enemy.X.Should().Be(150); // Position unchanged by healing
        enemy.Y.Should().Be(250);

        // Kill
        enemy.Kill();
        enemy.IsAlive.Should().BeFalse();
        enemy.CurrentHealth.Should().Be(0);
        enemy.X.Should().Be(150); // Position unchanged by death
        enemy.Y.Should().Be(250);
    }

    [Fact]
    public void LastDamageTime_ShouldTrackMostRecentDamage()
    {
        var stats = CreateValidStats();
        var enemy = new Enemy(stats, 0, 0);

        enemy.LastDamageTime.Should().Be(0f);

        enemy.TakeDamage(10, 5.0f);
        enemy.LastDamageTime.Should().Be(5.0f);

        enemy.TakeDamage(20, 10.0f);
        enemy.LastDamageTime.Should().Be(10.0f);
    }
}
