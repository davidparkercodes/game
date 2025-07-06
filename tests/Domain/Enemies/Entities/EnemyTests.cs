using System;
using FluentAssertions;
using Game.Domain.Enemies.Entities;
using Game.Domain.Enemies.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Enemies.Entities;

public class EnemyTests
{
    private static EnemyStats CreateTestStats() => 
        new EnemyStats(100, 2.0f, 5, 10, 20, "Test Enemy");

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEnemy()
    {
        var stats = CreateTestStats();
        var enemy = new Enemy(stats, 50.0f, 75.0f);

        enemy.Stats.Should().Be(stats);
        enemy.CurrentHealth.Should().Be(100);
        enemy.X.Should().Be(50.0f);
        enemy.Y.Should().Be(75.0f);
        enemy.IsAlive.Should().BeTrue();
    }

    [Fact]
    public void TakeDamage_ShouldReduceHealthAndTrackTime()
    {
        var stats = CreateTestStats();
        var enemy = new Enemy(stats, 0, 0);

        enemy.TakeDamage(30, 5.0f);

        enemy.CurrentHealth.Should().Be(70);
        enemy.LastDamageTime.Should().Be(5.0f);
        enemy.IsAlive.Should().BeTrue();
    }

    [Fact]
    public void TakeDamage_WhenFatalDamage_ShouldKillEnemy()
    {
        var stats = CreateTestStats();
        var enemy = new Enemy(stats, 0, 0);

        enemy.TakeDamage(100, 1.0f);

        enemy.CurrentHealth.Should().Be(0);
        enemy.IsAlive.Should().BeFalse();
    }

    [Fact]
    public void HealthPercentage_ShouldReflectCurrentHealth()
    {
        var stats = CreateTestStats();
        var enemy = new Enemy(stats, 0, 0);

        enemy.HealthPercentage.Should().Be(1.0f);

        enemy.TakeDamage(25, 1.0f);
        enemy.HealthPercentage.Should().Be(0.75f);
    }

    [Fact]
    public void CalculateDistance_ShouldReturnCorrectDistance()
    {
        var stats = CreateTestStats();
        var enemy = new Enemy(stats, 0, 0);

        var distance = enemy.CalculateDistance(3.0f, 4.0f);

        distance.Should().Be(5.0f); // 3-4-5 triangle
    }
}
