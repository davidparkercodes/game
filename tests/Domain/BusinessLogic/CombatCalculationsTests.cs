using FluentAssertions;
using Game.Domain.Buildings.ValueObjects;
using Game.Domain.Enemies.ValueObjects;
using Game.Domain.Enemies.Entities;
using Xunit;

namespace Game.Tests.Domain.BusinessLogic;

public class CombatCalculationsTests
{
    [Fact]
    public void BuildingStats_DamagePerSecond_ShouldCalculateCorrectly()
    {
        var stats = new BuildingStats(100, 30, 150.0f, 60.0f, 800.0f, "", "", "");

        var dps = stats.DamagePerSecond;

        dps.Should().Be(60.0f); // 30 damage * (60 attacks/sec / 30) = 60 DPS
    }

    [Fact]
    public void BuildingStats_CostEffectiveness_ShouldCalculateCorrectly()
    {
        var expensiveTower = new BuildingStats(200, 25, 150.0f, 60.0f, 800.0f, "", "", "");
        var cheapTower = new BuildingStats(100, 25, 150.0f, 60.0f, 800.0f, "", "", "");

        expensiveTower.CostEffectiveness.Should().Be(0.25f); // 50 DPS / 200 cost
        cheapTower.CostEffectiveness.Should().Be(0.5f); // 50 DPS / 100 cost
    }

    [Fact]
    public void Enemy_HealthPercentage_ShouldReflectCurrentHealth()
    {
        var stats = new EnemyStats(100, 2.0f, 5, 10, 20, "Test Enemy");
        var enemy = new Enemy(stats, 0, 0);

        enemy.HealthPercentage.Should().Be(1.0f);

        enemy.TakeDamage(75, 1.0f);
        enemy.HealthPercentage.Should().Be(0.25f);
    }

    [Fact]
    public void BossEnemy_IsInFinalPhase_ShouldActivateAt25PercentHealth()
    {
        var stats = new EnemyStats(1000, 5.0f, 10, 50, 100, "Boss");
        var boss = new BossEnemy(stats, 0, 0);

        boss.IsInFinalPhase.Should().BeFalse();

        boss.TakeDamage(750, 1.0f); // 250/1000 = 25%
        boss.IsInFinalPhase.Should().BeTrue();

        boss.TakeDamage(1, 1.0f); // 249/1000 < 25%
        boss.IsInFinalPhase.Should().BeTrue();
    }

    [Fact]
    public void Tower_AttackSpeed_ShouldDetermineShootingInterval()
    {
        var fastTowerStats = new BuildingStats(100, 20, 150.0f, 60.0f, 800.0f, "", "", "");
        var slowTowerStats = new BuildingStats(100, 40, 150.0f, 30.0f, 800.0f, "", "", "");

        var fastTowerInterval = 30.0f / fastTowerStats.AttackSpeed; // 0.5 seconds
        var slowTowerInterval = 30.0f / slowTowerStats.AttackSpeed; // 1.0 second

        fastTowerInterval.Should().Be(0.5f);
        slowTowerInterval.Should().Be(1.0f);

        // Verify DPS is same despite different attack patterns
        fastTowerStats.DamagePerSecond.Should().Be(40.0f); // 20 * 2 attacks/sec
        slowTowerStats.DamagePerSecond.Should().Be(40.0f); // 40 * 1 attack/sec
    }
}
