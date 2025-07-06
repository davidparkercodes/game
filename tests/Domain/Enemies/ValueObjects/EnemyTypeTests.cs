using System;
using FluentAssertions;
using Game.Domain.Enemies.ValueObjects;
using Xunit;

namespace Game.Tests.Domain.Enemies.ValueObjects;

public class EnemyTypeTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateEnemyType()
    {
        var enemyType = new EnemyType("enemy_basic", "BasicEnemy", "Basic Enemy", "Ground", 3);

        enemyType.InternalId.Should().Be("enemy_basic");
        enemyType.ConfigKey.Should().Be("BasicEnemy");
        enemyType.DisplayName.Should().Be("Basic Enemy");
        enemyType.Category.Should().Be("Ground");
        enemyType.Tier.Should().Be(3);
    }

    [Fact]
    public void Constructor_WithInvalidTier_ShouldThrowArgumentException()
    {
        var action = () => new EnemyType("enemy_basic", "config", "display", "category", 0);

        action.Should().Throw<ArgumentException>()
            .WithMessage("Tier must be at least 1*");
    }

    [Fact]
    public void Equals_ShouldBeBasedOnInternalId()
    {
        var type1 = new EnemyType("enemy_basic", "Config1", "Display1", "Category1", 1);
        var type2 = new EnemyType("enemy_basic", "Config2", "Display2", "Category2", 5);
        var type3 = new EnemyType("enemy_boss", "Config1", "Display1", "Category1", 1);

        type1.Should().Be(type2);
        type1.Should().NotBe(type3);
    }
}
