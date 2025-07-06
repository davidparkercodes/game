using System;
using FluentAssertions;
using Game.Domain.Enemies.Entities;
using Game.Domain.Enemies.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Enemies.Entities;

public class BossEnemyTests
{
    private static EnemyStats CreateTestStats() => 
        new EnemyStats(1000, 5.0f, 10, 50, 100, "Boss Enemy");

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateBossEnemy()
    {
        var stats = CreateTestStats();
        var boss = new BossEnemy(stats, 100.0f, 200.0f, 3.0f);

        boss.Stats.Should().Be(stats);
        boss.ScaleMultiplier.Should().Be(3.0f);
        boss.CurrentHealth.Should().Be(1000);
        boss.IsAlive.Should().BeTrue();
        boss.IsImmuneToDamage.Should().BeFalse();
    }

    [Fact]
    public void TakeDamage_WhenImmune_ShouldNotTakeDamage()
    {
        var stats = CreateTestStats();
        var boss = new BossEnemy(stats, 0, 0);
        boss.ActivateDamageImmunity(5);

        boss.TakeDamage(500, 1.0f);

        boss.CurrentHealth.Should().Be(1000);
    }

    [Fact]
    public void TakeDamage_WhenNotImmune_ShouldTakeDamage()
    {
        var stats = CreateTestStats();
        var boss = new BossEnemy(stats, 0, 0);

        boss.TakeDamage(300, 1.0f);

        boss.CurrentHealth.Should().Be(700);
    }

    [Fact]
    public void IsInFinalPhase_WhenHealthLow_ShouldReturnTrue()
    {
        var stats = CreateTestStats();
        var boss = new BossEnemy(stats, 0, 0);
        boss.TakeDamage(800, 1.0f); // Health at 200/1000 = 20%

        boss.IsInFinalPhase.Should().BeTrue();
    }

    [Fact]
    public void UseSpecialAbility_ShouldActivateImmunity()
    {
        var stats = CreateTestStats();
        var boss = new BossEnemy(stats, 0, 0);

        boss.UseSpecialAbility();

        boss.IsImmuneToDamage.Should().BeTrue();
        boss.CanUseSpecialAbility.Should().BeFalse();
    }
}
